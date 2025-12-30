using System;
using System.IO;
using Oxide.Core.Configuration;
using Oxide.Core.Plugins;

namespace Oxide.Plugins;

/// <summary>
/// A config for a plugin that provides useful helper methods.
/// </summary>
/// <typeparam name="T">The data type this config stores.</typeparam>
internal abstract class PluginConfig<T> where T : new()
{
    private readonly Plugin plugin;
    private DynamicConfigFile? file;
    private readonly string? filename;

    /// <summary>
    /// The values stored in the config.
    /// </summary>
    public T? Values { get; private set; }

    /// <summary>
    /// Create a new config.
    /// </summary>
    /// <param name="plugin">The plugin this config is for.</param>
    /// <param name="filename">The filename of the config file.</param>
    protected PluginConfig(Plugin plugin, string? filename = null)
    {
        this.plugin = plugin;
        this.filename = filename;
    }

    /// <summary>
    /// Initialize the config.
    /// Should be called from the plugin's Init() hook.
    /// </summary>
    public void Init()
    {
        file = new DynamicConfigFile(GetFilepath(plugin, filename));
        Save();
    }

    /// <summary>
    /// Save the config to its file.
    /// </summary>
    public void Save()
    {
        if (file == null)
            throw new InvalidOperationException("File not initialized. Did you forget to call Init()?");

        if (Values == null)
            Load();
        else
            file.WriteObject(Values);
    }

    /// <summary>
    /// Load the config from its file.
    /// </summary>
    public void Load()
    {
        if (file == null)
            throw new InvalidOperationException("File not initialized. Did you forget to call Init()?");

        Values = file.ReadObject<T>();
        if (Values != null)
            return;

        Values = new T();
        Save();
    }

    /// <summary>
    /// Get the file path for the config file.
    /// </summary>
    public static string GetFilepath(Plugin plugin, string? filename = null)
    {
        Debug.Assert(!string.IsNullOrEmpty(plugin.Manager?.ConfigPath));
        Debug.Assert(!string.IsNullOrEmpty(plugin.Name));
        return string.IsNullOrEmpty(filename)
            ? Path.Combine(plugin.Manager.ConfigPath, plugin.Name + ".json") // $"oxide/config/{plugin.Name}.json"
            : Path.Combine(plugin.Manager.ConfigPath, plugin.Name,
                filename + ".json"); // $"oxide/config/{plugin.Name}/{shortFilename}.json"
    }
}