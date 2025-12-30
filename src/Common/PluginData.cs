using Oxide.Core;
using Oxide.Core.Plugins;

namespace Oxide.Plugins;

/// <summary>
/// A data file for a plugin that provides useful helper methods.
/// </summary>
/// <typeparam name="T">The data type this data file stores.</typeparam>
internal abstract class PluginData<T> where T : new()
{
    private readonly string filename;

    /// <summary>
    /// The values stored in the data file.
    /// </summary>
    public T? Values { get; protected set; }

    /// <summary>
    /// Create a new data file.
    /// </summary>
    /// <param name="plugin">The plugin this data file is for.</param>
    /// <param name="filename">The filename of the data file.</param>
    protected PluginData(Plugin plugin, string? filename = null)
    {
        this.filename = string.IsNullOrEmpty(filename) ? plugin.Name : $"{plugin.Name}/{filename}";
    }

    /// <summary>
    /// Save the data to its file.
    /// </summary>
    public void Save()
    {
        if (Values == null)
            Load();
        else
            Interface.Oxide.DataFileSystem.WriteObject(filename, Values);
    }

    /// <summary>
    /// Load the data from its file.
    /// </summary>
    public void Load()
    {
        Values = Interface.Oxide.DataFileSystem.ReadObject<T>(filename);
        if (Values == null)
        {
            Values = new T();
            Save();
        }
    }
}