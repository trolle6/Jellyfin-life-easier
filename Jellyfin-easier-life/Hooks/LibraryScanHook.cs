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

    /// <summary>
    /// Initializes a new instance of the <see cref="LibraryScanHook"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="libraryManager">The library manager.</param>
    /// <param name="metadataReplacementService">The metadata replacement service.</param>
    /// <param name="plugin">The plugin instance.</param>
    public LibraryScanHook(
        ILogger<LibraryScanHook> logger,
        ILibraryManager libraryManager,
        MetadataReplacementService metadataReplacementService,
        Plugin plugin)
    {
        _logger = logger;
        _libraryManager = libraryManager;
        _metadataReplacementService = metadataReplacementService;
        _plugin = plugin;
    }

    /// <inheritdoc />
    public Task RunAsync()
    {
        // Hook into library scan events
        _libraryManager.ItemAdded += OnItemAdded;
        _libraryManager.ItemUpdated += OnItemUpdated;
        
        _logger.LogInformation("Library scan hook initialized. Metadata replacement is {Status}.",
            _plugin.Configuration.ReplaceAllMetadata ? "enabled" : "disabled");

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
                _logger.LogDebug("Item added: {ItemName}, triggering metadata replacement", e.Item.Name);
                await _metadataReplacementService.ReplaceMetadataAsync(
                    e.Item,
                    _plugin.Configuration.ReplaceImages,
                    CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OnItemAdded hook for item: {ItemName}", e.Item?.Name);
        }
    }

    private async void OnItemUpdated(object? sender, ItemChangeEventArgs e)
    {
        if (!_plugin.Configuration.ReplaceAllMetadata || !_plugin.Configuration.ForceMetadataRefresh)
        {
            return;
        }

        try
        {
            if (e.Item != null && e.UpdateReason == ItemUpdateType.MetadataDownload)
            {
                _logger.LogDebug("Item metadata updated: {ItemName}, triggering full metadata replacement", e.Item.Name);
                await _metadataReplacementService.ReplaceMetadataAsync(
                    e.Item,
                    _plugin.Configuration.ReplaceImages,
                    CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OnItemUpdated hook for item: {ItemName}", e.Item?.Name);
        }
    }
}

