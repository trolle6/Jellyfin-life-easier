using MediaBrowser.Model.Plugins;

namespace Jellyfin_easier_life;

/// <summary>
/// Plugin configuration options.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether to replace all metadata during library scans.
    /// </summary>
    public bool ReplaceAllMetadata { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to combine all seasons into one season.
    /// </summary>
    public bool CombineAllSeasons { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to force metadata refresh even if metadata already exists.
    /// </summary>
    public bool ForceMetadataRefresh { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to replace images during metadata refresh.
    /// </summary>
    public bool ReplaceImages { get; set; } = true;
}

