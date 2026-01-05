using Jellyfin_easier_life.Services;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Plugins;
using Microsoft.Extensions.Logging;

namespace Jellyfin_easier_life.Hooks;

/// <summary>
/// Hook to intercept library scan operations and replace with full metadata refresh.
/// </summary>
public class LibraryScanHook : IServerEntryPoint
{
    private readonly ILogger<LibraryScanHook> _logger;
    private readonly ILibraryManager _libraryManager;
    private readonly MetadataReplacementService _metadataReplacementService;
    private readonly Plugin _plugin;
    private readonly PluginActivityTracker? _activityTracker;

    /// <summary>
    /// Initializes a new instance of the <see cref="LibraryScanHook"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="libraryManager">The library manager.</param>
    /// <param name="metadataReplacementService">The metadata replacement service.</param>
    /// <param name="plugin">The plugin instance.</param>
    /// <param name="activityTracker">The activity tracker (optional).</param>
    public LibraryScanHook(
        ILogger<LibraryScanHook> logger,
        ILibraryManager libraryManager,
        MetadataReplacementService metadataReplacementService,
        Plugin plugin,
        PluginActivityTracker? activityTracker = null)
    {
        _logger = logger;
        _libraryManager = libraryManager;
        _metadataReplacementService = metadataReplacementService;
        _plugin = plugin;
        _activityTracker = activityTracker;
    }

    /// <inheritdoc />
    public Task RunAsync()
    {
        // Hook into library scan events
        _libraryManager.ItemAdded += OnItemAdded;
        _libraryManager.ItemUpdated += OnItemUpdated;
        
        // Log with clear, visible markers
        if (_plugin.Configuration.ReplaceAllMetadata)
        {
            _logger.LogWarning("✅ [Jellyfin Easier Life] PLUGIN ACTIVE - Metadata replacement is ENABLED");
            _logger.LogWarning("   → All library refreshes will automatically replace ALL metadata");
            _logger.LogWarning("   → Replace Images: {ReplaceImages}", _plugin.Configuration.ReplaceImages);
        }
        else
        {
            _logger.LogInformation("[Jellyfin Easier Life] Plugin loaded but metadata replacement is DISABLED");
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _libraryManager.ItemAdded -= OnItemAdded;
        _libraryManager.ItemUpdated -= OnItemUpdated;
    }

    private async void OnItemAdded(object? sender, ItemChangeEventArgs e)
    {
        if (!_plugin.Configuration.ReplaceAllMetadata)
        {
            return;
        }

        try
        {
            if (e.Item != null)
            {
                _logger.LogWarning("🆕 [Jellyfin Easier Life] NEW ITEM DETECTED: {ItemName} - Triggering full metadata replacement", e.Item.Name);
                await _metadataReplacementService.ReplaceMetadataAsync(
                    e.Item,
                    _plugin.Configuration.ReplaceImages,
                    CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [Jellyfin Easier Life] Error in OnItemAdded hook for item: {ItemName}", e.Item?.Name);
        }
    }

    private async void OnItemUpdated(object? sender, ItemChangeEventArgs e)
    {
        if (!_plugin.Configuration.ReplaceAllMetadata)
        {
            return;
        }

        try
        {
            if (e.Item != null)
            {
                // Be VERY aggressive - intercept ALL updates when ForceMetadataRefresh is enabled
                // This ensures "Refresh Library" becomes "Replace All Metadata" automatically
                bool shouldReplace = false;

                if (_plugin.Configuration.ForceMetadataRefresh)
                {
                    // Catch ALL update types - library refresh can trigger any of these
                    // This is the key to making "Refresh Library" work as "Replace All Metadata"
                    shouldReplace = e.UpdateReason == ItemUpdateType.MetadataDownload || 
                                   e.UpdateReason == ItemUpdateType.MetadataEdit ||
                                   e.UpdateReason == ItemUpdateType.ImageUpdate ||
                                   e.UpdateReason == ItemUpdateType.None || // Library refresh often uses None
                                   e.UpdateReason == ItemUpdateType.UserDataSaved ||
                                   e.UpdateReason == ItemUpdateType.ChapterMetadataDownloaded;
                }
                else
                {
                    // Only catch explicit metadata downloads
                    shouldReplace = e.UpdateReason == ItemUpdateType.MetadataDownload;
                }

                if (shouldReplace)
                {
                    _logger.LogWarning("🔄 [Jellyfin Easier Life] ITEM UPDATED: {ItemName} (Reason: {Reason}) - Intercepting and replacing ALL metadata", 
                        e.Item.Name, e.UpdateReason);
                    
                    // ALWAYS replace all metadata and images when plugin is enabled
                    // This is the core functionality - turning soft scan into full replacement
                    await _metadataReplacementService.ReplaceMetadataAsync(
                        e.Item,
                        _plugin.Configuration.ReplaceImages, // Both boxes checked automatically
                        CancellationToken.None);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [Jellyfin Easier Life] Error in OnItemUpdated hook for item: {ItemName}", e.Item?.Name);
        }
    }
}

