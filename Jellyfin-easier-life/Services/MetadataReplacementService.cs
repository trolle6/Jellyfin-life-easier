using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.IO;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using Microsoft.Extensions.Logging;

namespace Jellyfin_easier_life.Services;

/// <summary>
/// Service for replacing metadata during library scans.
/// </summary>
public class MetadataReplacementService
{
    private readonly ILogger<MetadataReplacementService> _logger;
    private readonly ILibraryManager _libraryManager;
    private readonly IProviderManager _providerManager;
    private readonly IDirectoryService _directoryService;
    private readonly PluginActivityTracker? _activityTracker;

    /// <summary>
    /// Initializes a new instance of the <see cref="MetadataReplacementService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="libraryManager">The library manager.</param>
    /// <param name="providerManager">The provider manager.</param>
    /// <param name="directoryService">The directory service.</param>
    /// <param name="activityTracker">The activity tracker (optional).</param>
    public MetadataReplacementService(
        ILogger<MetadataReplacementService> logger,
        ILibraryManager libraryManager,
        IProviderManager providerManager,
        IDirectoryService directoryService,
        PluginActivityTracker? activityTracker = null)
    {
        _logger = logger;
        _libraryManager = libraryManager;
        _providerManager = providerManager;
        _directoryService = directoryService;
        _activityTracker = activityTracker;
    }

    /// <summary>
    /// Replaces all metadata for a given item.
    /// </summary>
    /// <param name="item">The item to refresh metadata for.</param>
    /// <param name="replaceImages">Whether to replace images.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task ReplaceMetadataAsync(
        BaseItem item,
        bool replaceImages = true,
        CancellationToken cancellationToken = default)
    {
        bool success = false;
        try
        {
            // Log with clear prefix so it's easy to spot in logs
            _logger.LogWarning("🔄 [Jellyfin Easier Life] REPLACING ALL METADATA for: {ItemName} ({ItemType}, ID: {ItemId})", 
                item.Name, item.GetType().Name, item.Id);

            // Force metadata refresh - ALWAYS replace all metadata and images when called
            // This ensures "Refresh Library" becomes "Replace All Metadata" automatically
            var refreshOptions = new MetadataRefreshOptions(_directoryService)
            {
                ReplaceAllMetadata = true,  // Always true - this is the whole point!
                ReplaceImages = replaceImages,  // Controlled by plugin config
                MetadataRefreshMode = MetadataRefreshMode.FullRefresh,  // Full refresh, not soft scan
                ImageRefreshMode = replaceImages ? MetadataRefreshMode.FullRefresh : MetadataRefreshMode.ValidationOnly
            };

            await _providerManager.RefreshFullMetadata(item, refreshOptions, cancellationToken);

            success = true;
            _logger.LogWarning("✅ [Jellyfin Easier Life] SUCCESSFULLY REPLACED metadata for: {ItemName}", item.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [Jellyfin Easier Life] ERROR replacing metadata for: {ItemName} ({ItemId})", item.Name, item.Id);
            throw;
        }
        finally
        {
            _activityTracker?.RecordItemProcessed(item, success);
        }
    }

    /// <summary>
    /// Replaces metadata for all items in a library.
    /// </summary>
    /// <param name="libraryId">The library ID.</param>
    /// <param name="replaceImages">Whether to replace images.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task ReplaceMetadataForLibraryAsync(
        Guid libraryId,
        bool replaceImages = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting metadata replacement for library: {LibraryId}", libraryId);

            var rootFolder = _libraryManager.GetItemById(libraryId);
            if (rootFolder == null)
            {
                _logger.LogWarning("Library not found: {LibraryId}", libraryId);
                return;
            }

            var items = rootFolder.GetRecursiveChildren().ToList();
            _logger.LogInformation("Found {Count} items to process", items.Count);

            foreach (var item in items)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Metadata replacement cancelled");
                    break;
                }

                await ReplaceMetadataAsync(item, replaceImages, cancellationToken);
            }

            _logger.LogInformation("Completed metadata replacement for library: {LibraryId}", libraryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error replacing metadata for library: {LibraryId}", libraryId);
            throw;
        }
    }
}

