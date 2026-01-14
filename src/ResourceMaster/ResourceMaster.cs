using System.Collections.Generic;
using System.IO;
using System.Linq;
using Oxide.Core;
using Oxide.Core.Configuration;
using UnityEngine;

namespace Oxide.Plugins;

[Info("Resource Master", "BlueBeka", "0.0.1")]
[Description("Change the amount or even kind of resource gained from gathering resources.")]
public partial class ResourceMaster : RustPlugin
{
    private static ResourceMaster? self;

    private bool serverInitialized;
    private readonly UserConfig config;
    private readonly DispenserRates moddedRates;
    private readonly DispenserRates vanillaRates;

    private const string wildCard = "*";

    /// <summary>
    /// Create a new instance of the plugin.
    /// </summary>
    public ResourceMaster()
    {
        Panic.If(self != null);
        self = this;
        serverInitialized = false;

        config = new UserConfig();
        moddedRates = new DispenserRates(DispenserRates.moddedRatesFilename);
        vanillaRates = new DispenserRates(DispenserRates.vanillaRatesFilename);
    }

    #region Core Logic

    /// <summary>
    /// Make sure we have the given dispenser recorded.
    /// </summary>
    private void EnsureRecorded(ResourceDispenser dispenser)
    {
        string prefab = dispenser.gameObject.ToBaseEntity().PrefabName;
        Panic.If(string.IsNullOrEmpty(prefab));

        // Don't already have this item?
        if (!vanillaRates.dispensers.ContainsKey(prefab))
        {
            vanillaRates.dispensers.Add(prefab, new DispenserData());

            foreach (var itemAmount in dispenser.containedItems)
            {
                Panic.IfNull(itemAmount.itemDef);
                Panic.If(string.IsNullOrEmpty(itemAmount.itemDef!.shortname));
                Panic.If(itemAmount.startAmount < 0);
                vanillaRates.dispensers[prefab].containedItems
                    .Add(itemAmount.itemDef.shortname, itemAmount.startAmount);
            }

            foreach (var itemAmount in dispenser.finishBonus)
            {
                Panic.If(itemAmount.itemDef != null);
                Panic.If(string.IsNullOrEmpty(itemAmount.itemDef!.shortname));
                Panic.If(itemAmount.startAmount < 0);
                vanillaRates.dispensers[prefab].finishBonus.Add(itemAmount.itemDef.shortname, itemAmount.startAmount);
            }
        }

        GenerateModdedRates(prefab);
    }

    /// <summary>
    /// Generate what the rates for the given prefab should be based on the configs.
    /// </summary>
    private void GenerateModdedRates(string prefab)
    {
        // Already have this item?
        if (moddedRates.dispensers.ContainsKey(prefab))
            return;

        if (config.overrides.Values?.overrides.dispensers.ContainsKey(prefab) == true)
        {
            // TODO: Handle overrides.
        }
        else if (vanillaRates.dispensers.ContainsKey(prefab))
        {
            foreach (var item in vanillaRates.dispensers[prefab].containedItems)
            {
                float rateModifer = config.modifiers.Values?.resourceDispensers.ContainsKey(item.Key) == true
                    ? config.modifiers.Values.resourceDispensers[item.Key]
                    : config.modifiers.Values?.resourceDispensers.ContainsKey(wildCard) == true
                        ? config.modifiers.Values.resourceDispensers[wildCard]
                        : 1f;

                if (rateModifer < 0)
                {
                    Interface.Oxide.LogWarning(
                        $"Resource rate modifier for the containedItems of \"{prefab}\" is {rateModifer}, which is < 0. This will not be applied.");
                    continue;
                }

                if (!moddedRates.dispensers.ContainsKey(prefab))
                    moddedRates.dispensers.Add(prefab, new DispenserData());

                if (rateModifer == 0)
                    moddedRates.dispensers[prefab].containedItems.Remove(item.Key);
                else
                    moddedRates.dispensers[prefab].containedItems.Add(item.Key, item.Value * rateModifer);
            }

            foreach (var item in vanillaRates.dispensers[prefab].finishBonus)
            {
                float rateModifer = config.modifiers.Values?.resourceDispensers.ContainsKey(item.Key) == true
                    ? config.modifiers.Values.resourceDispensers[item.Key]
                    : config.modifiers.Values?.resourceDispensers.ContainsKey(wildCard) == true
                        ? config.modifiers.Values.resourceDispensers[wildCard]
                        : 1f;

                if (rateModifer < 0)
                {
                    Interface.Oxide.LogWarning(
                        $"Resource rate modifier for the finishBonus of \"{prefab}\" is {rateModifer}, which is < 0. This will not be applied.");
                    continue;
                }

                if (!moddedRates.dispensers.ContainsKey(prefab))
                    moddedRates.dispensers.Add(prefab, new DispenserData());

                if (rateModifer == 0)
                    moddedRates.dispensers[prefab].finishBonus.Remove(item.Key);
                else
                    moddedRates.dispensers[prefab].finishBonus.Add(item.Key, item.Value * rateModifer);
            }
        }
    }

    /// <summary>
    /// Sets the resource on the all dispensers to the modded amounts.
    /// </summary>
    private void SetModdedResourceRates()
    {
        SetResourceRates(moddedRates);
    }

    /// <summary>
    /// Sets the resource on the given dispenser to the modded amounts.
    /// </summary>
    /// <returns>"true" if the dispenser was updated, "false" if not change was needed.</returns>
    private bool SetModdedResourceRate(ResourceDispenser dispenser)
    {
        return SetResourceRate(dispenser, moddedRates);
    }

    /// <summary>
    /// Restore the resources on all dispensers back to vanilla amounts.
    /// </summary>
    private void RestoreResourceRates()
    {
        SetResourceRates(vanillaRates);
    }

    /// <summary>
    /// Restore the resources on the given dispenser back to vanilla amounts.
    /// </summary>
    /// <returns>"true" if the dispenser was updated, "false" if not change was needed.</returns>
    private bool RestoreResourceRate(ResourceDispenser dispenser)
    {
        return SetResourceRate(dispenser, vanillaRates);
    }

    /// <summary>
    /// Sets the resource on the all dispensers to the given rate.
    /// Note: This is an expensive operation.
    /// </summary>
    private void SetResourceRates(DispenserRates rates)
    {
        var dispensers = BaseNetworkable.serverEntities
            .Select(entity => entity.GetComponent<ResourceDispenser>())
            .Where(dispenser => dispenser != null);

        int updates = 0;
        foreach (var dispenser in dispensers)
            if (SetResourceRate(dispenser, rates))
                updates++;

        Interface.Oxide.LogInfo(
            "Updated {0} resource dispensers of {1}.",
            updates,
            dispensers.Count()
        );
    }

    /// <summary>
    /// Set the resources on the given dispenser back to the given rate.
    /// </summary>
    /// <returns>"true" if the dispenser was updated, "false" if not change was needed.</returns>
    private bool SetResourceRate(ResourceDispenser dispenser, DispenserRates rates)
    {
        string prefab = dispenser.gameObject.ToBaseEntity().PrefabName;
        Panic.If(string.IsNullOrEmpty(prefab));

        List<(string Category, ItemAmount Item)> resources = new();
        resources.AddRange(dispenser.containedItems.Select(item => ("containedItems", item)));
        resources.AddRange(dispenser.finishBonus.Select(item => ("finishBonus", item)));

        bool changeMade = false;

        foreach (var resource in dispenser.containedItems)
        {
            if (!rates.dispensers.ContainsKey(prefab) ||
                !rates.dispensers[prefab].containedItems.ContainsKey(resource.itemDef.shortname))
            {
                Interface.Oxide.LogWarning("Missing rates for \"{0}[{containedItems}][{1}]\".", prefab,
                    resource.itemDef.shortname);
                continue;
            }

            var newStartAmount = rates.dispensers[prefab].containedItems[resource.itemDef.shortname];
            if (newStartAmount < 0)
            {
                Interface.Oxide.LogWarning(
                    "Resource rate modifier for the containedItems of \"{0}\" is {1}, which is < 0. This will not be applied.",
                    prefab, newStartAmount);
                continue;
            }

            if (resource.startAmount != newStartAmount)
            {
                resource.startAmount = newStartAmount;
                resource.amount = newStartAmount * dispenser.fractionRemaining;
                changeMade = true;
            }
        }

        foreach (var resource in dispenser.finishBonus)
        {
            if (!rates.dispensers.ContainsKey(prefab) ||
                !rates.dispensers[prefab].finishBonus.ContainsKey(resource.itemDef.shortname))
            {
                Interface.Oxide.LogWarning("Missing rates for \"{0}[finishBonus][{1}]\".", prefab,
                    resource.itemDef.shortname);
                continue;
            }

            var newStartAmount = rates.dispensers[prefab].finishBonus[resource.itemDef.shortname];
            if (newStartAmount < 0)
            {
                Interface.Oxide.LogWarning(
                    "Resource rate modifier for the finishBonus of \"{0}\" is {1}, which is < 0. This will not be applied.",
                    prefab, newStartAmount);
                continue;
            }

            if (resource.startAmount != newStartAmount)
            {
                resource.startAmount = newStartAmount;
                resource.amount = newStartAmount * dispenser.fractionRemaining;
                changeMade = true;
            }
        }

        if (changeMade)
            dispenser.Initialize();

        return changeMade;
    }

    #endregion

    #region Hooks

    private void Init()
    {
        config.Init();
    }

    private void Loaded()
    {
        config.Load();
        vanillaRates.Load();
        moddedRates.Load();
    }


    private void OnServerInitialized()
    {
        IEnumerable<ResourceDispenser> dispensers = BaseNetworkable.serverEntities
            .Select(entity => entity.GetComponent<ResourceDispenser>())
            .Where(dispenser => dispenser != null);

        foreach (var dispenser in dispensers)
            EnsureRecorded(dispenser);

        config.Init();
        SetModdedResourceRates();

        serverInitialized = true;
    }

    void OnServerSave()
    {
        if (!serverInitialized)
            return;

        vanillaRates.Save();
        moddedRates.Save();
    }

    private void Unload()
    {
        if (!serverInitialized)
            return;

        RestoreResourceRates();
        self = null;
        serverInitialized = false;
    }

    private void OnEntitySpawned(BaseNetworkable entity)
    {
        if (!serverInitialized)
            return;

        ResourceDispenser? dispenser = entity.GetComponent<ResourceDispenser>();
        if (dispenser == null)
            return;

        EnsureRecorded(dispenser);
        SetModdedResourceRate(dispenser);
    }

    #endregion

    #region Data

    /// <summary>
    /// All the config for this plugin.
    /// </summary>
    private class UserConfig
    {
        public ModifiersConfig modifiers { get; } = new ModifiersConfig();
        public OverridesConfig overrides { get; } = new OverridesConfig();

        /// <summary>
        /// Initialize the config.
        /// </summary>
        public void Init()
        {
            modifiers.Init();
            overrides.Init();
        }

        /// <summary>
        /// Save all the config data.
        /// </summary>
        public void Save()
        {
            modifiers.Save();
            overrides.Save();
        }

        /// <summary>
        /// Load all the config data.
        /// </summary>
        public void Load()
        {
            modifiers.Load();
            overrides.Load();
        }

        /// <summary>
        /// Get the filename path for the config file.
        /// </summary>
        private static string GetFilename(string? shortFilename)
        {
            Panic.IfNull(self?.Manager);
            Panic.If(string.IsNullOrEmpty(self.Manager!.ConfigPath));
            Panic.If(string.IsNullOrEmpty(self.Name));

            return string.IsNullOrEmpty(shortFilename)
                ? Path.Combine(self.Manager.ConfigPath, self.Name + ".json") // $"oxide/config/{self.Name}.json"
                : Path.Combine(self.Manager.ConfigPath, self.Name,
                    shortFilename + ".json"); // $"oxide/config/{self.Name}/{file}.json"
        }

        public abstract class PluginConfig<T> where T : new()
        {
            private DynamicConfigFile? file;
            private readonly string? shortFilename;

            public T? Values { get; private set; }


            public PluginConfig(string? filename)
            {
                shortFilename = filename;
            }

            public void Init()
            {
                file = new DynamicConfigFile(GetFilename(shortFilename));
                Save();
            }

            public void Save()
            {
                Panic.IfNull(file);
                if (Values == null)
                    Load();
                else
                    file!.WriteObject(Values, false);
            }

            public void Load()
            {
                Panic.IfNull(file);
                Values = file!.ReadObject<T>();
                if (Values == null)
                {
                    Values = InitializeData();
                    Panic.IfNull(Values);
                    Save();
                }
            }

            /// <summary>
            /// Get the filename path for the config file.
            /// </summary>
            private static string GetFilename(string? shortFilename)
            {
                Panic.IfNull(self);
                return string.IsNullOrEmpty(shortFilename)
                    ? Path.Combine(self!.Manager.ConfigPath, self.Name + ".json") // $"oxide/config/{self.Name}.json"
                    : Path.Combine(self!.Manager.ConfigPath, self.Name,
                        shortFilename + ".json"); // $"oxide/config/{self.Name}/{file}.json"
            }

            protected virtual T InitializeData()
            {
                return new T();
            }
        }

        /// <summary>
        /// The modifier config for this plugin.
        /// </summary>
        public class ModifiersConfig : PluginConfig<ModifiersConfig.Data>
        {
            public ModifiersConfig() : base("Modifiers")
            {
            }

            public class Data
            {
                public Dictionary<string, float> resourceDispensers = new Dictionary<string, float>()
                    { { wildCard, 1.0f } };

                public Dictionary<string, float> resourcePickups = new Dictionary<string, float>()
                    { { wildCard, 1.0f } };

                public Dictionary<string, float> surveyChargeExplo = new Dictionary<string, float>()
                    { { wildCard, 1.0f } };

                public QuarriesDef miningQuarries = new QuarriesDef();

                public class QuarriesDef
                {
                    public QuarryOptions stone = new QuarryOptions();
                    public QuarryOptions highQuality = new QuarryOptions();
                    public QuarryOptions sulfur = new QuarryOptions();
                    public QuarryOptions playerPlaced = new QuarryOptions();

                    public class QuarryOptions
                    {
                        public Dictionary<string, float> modifiers = new Dictionary<string, float>()
                            { { wildCard, 1.0f } };

                        public float tickRate = 5.0f;
                    };
                };

                public PumpJacksDef PumpJacks = new PumpJacksDef();

                public class PumpJacksDef
                {
                    public QuarriesDef.QuarryOptions Monument = new QuarriesDef.QuarryOptions();
                    public QuarriesDef.QuarryOptions PlayerPlaced = new QuarriesDef.QuarryOptions();
                };

                public ExcavatorDef Excavator = new ExcavatorDef();

                public class ExcavatorDef
                {
                    public float TickRate = 5.0f;
                    public float TimeForFullResources = 120.0f;
                }
            }
        }

        public class OverridesConfig : PluginConfig<OverridesConfig.Data>
        {
            public OverridesConfig() : base("Overrides")
            {
            }

            public class Data
            {
                public OverridesDef overrides = new OverridesDef();

                public class OverridesDef
                {
                    public Dictionary<string, DispensersDef> dispensers = new Dictionary<string, DispensersDef>();

                    public class DispensersDef
                    {
                        public Dictionary<string, float> containedItems = new Dictionary<string, float>();
                        public Dictionary<string, float> finishBonus = new Dictionary<string, float>();
                    }

                    public Dictionary<string, Dictionary<string, float>> pickup =
                        new Dictionary<string, Dictionary<string, float>>();

                    public Dictionary<string, float> survey = new Dictionary<string, float>();
                }
            }
        }
    }

    private class DispenserRates
    {
        // The filename for the data files.
        public const string moddedRatesFilename = "ModdedDispenserRates";
        public const string vanillaRatesFilename = "VanillaDispenserRates";

        // The filename for this instance.
        private readonly string filename;

        // Dispensers data.
        public Dictionary<string, DispenserData> dispensers { get; protected set; }

        public DispenserRates(string? file = null)
        {
            Panic.IfNull(self);
            filename = string.IsNullOrEmpty(file) ? self!.Name : $"{self!.Name}/{file}";
            dispensers = new Dictionary<string, DispenserData>();
        }

        /// <summary>
        /// Save the data.
        /// </summary>
        public void Save()
        {
            Interface.Oxide.DataFileSystem.WriteObject(filename, dispensers);
        }

        /// <summary>
        /// Load the data.
        /// </summary>
        public void Load()
        {
            dispensers = Interface.Oxide.DataFileSystem.ReadObject<Dictionary<string, DispenserData>>(filename);
        }
    }

    public class DispenserData
    {
        public Dictionary<string, float> containedItems { get; } = new Dictionary<string, float>();
        public Dictionary<string, float> finishBonus { get; } = new Dictionary<string, float>();
    }

    #endregion
}
