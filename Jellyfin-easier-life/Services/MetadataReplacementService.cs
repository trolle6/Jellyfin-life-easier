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

    /// <summary>
    /// Initializes a new instance of the <see cref="MetadataReplacementService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="libraryManager">The library manager.</param>
    /// <param name="providerManager">The provider manager.</param>
    /// <param name="directoryService">The directory service.</param>
    public MetadataReplacementService(
        ILogger<MetadataReplacementService> logger,
        ILibraryManager libraryManager,
        IProviderManager providerManager,
        IDirectoryService directoryService)
    {
        _logger = logger;
        _libraryManager = libraryManager;
        _providerManager = providerManager;
        _directoryService = directoryService;
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
        try
        {
            _logger.LogInformation("Replacing metadata for item: {ItemName} ({ItemId})", item.Name, item.Id);

            // Force metadata refresh by clearing existing metadata
            var refreshOptions = new MetadataRefreshOptions(_directoryService)
            {
                ReplaceAllMetadata = true,
                ReplaceImages = replaceImages,
                MetadataRefreshMode = MetadataRefreshMode.FullRefresh,
                ImageRefreshMode = replaceImages ? MetadataRefreshMode.FullRefresh : MetadataRefreshMode.ValidationOnly
            };

            await _providerManager.RefreshFullMetadata(item, refreshOptions, cancellationToken);

            _logger.LogInformation("Successfully replaced metadata for item: {ItemName}", item.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error replacing metadata for item: {ItemName} ({ItemId})", item.Name, item.Id);
            throw;
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

