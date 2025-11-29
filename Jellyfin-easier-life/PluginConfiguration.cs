using MediaBrowser.Model.Plugins;

namespace Jellyfin_easier_life;

/// <summary>
/// Plugin configuration options.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether to replace all metadata during library scans.
    /// When enabled, "Refresh Library" automatically becomes "Replace All Metadata" for all items.
    /// </summary>
    public bool ReplaceAllMetadata { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to combine all seasons into one season.
    /// </summary>
    public bool CombineAllSeasons { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to force metadata refresh even if metadata already exists.
    /// This ensures all items get refreshed during library scans, not just new ones.
    /// </summary>
    public bool ForceMetadataRefresh { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to replace images during metadata refresh.
    /// When enabled along with ReplaceAllMetadata, images are automatically replaced for all items.
    /// </summary>
    public bool ReplaceImages { get; set; } = true;
}
