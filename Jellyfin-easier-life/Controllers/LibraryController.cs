using Jellyfin.Controller;
using Jellyfin_easier_life.Services;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin_easier_life.Controllers;

/// <summary>
/// Controller for enhanced library operations.
/// </summary>
[ApiController]
[Route("Library")]
[Authorize(Policy = "DefaultAuthorization")]
public class LibraryController : BaseJellyfinApiController
{
    private readonly ILibraryManager _libraryManager;
    private readonly MetadataReplacementService _metadataReplacementService;
    private readonly SeasonCombinationService _seasonCombinationService;
    private readonly Plugin _plugin;
    private readonly PluginActivityTracker? _activityTracker;

    /// <summary>
    /// Initializes a new instance of the <see cref="LibraryController"/> class.
    /// </summary>
    /// <param name="libraryManager">The library manager.</param>
    /// <param name="metadataReplacementService">The metadata replacement service.</param>
    /// <param name="seasonCombinationService">The season combination service.</param>
    /// <param name="plugin">The plugin instance.</param>
    /// <param name="activityTracker">The activity tracker (optional).</param>
    public LibraryController(
        ILibraryManager libraryManager,
        MetadataReplacementService metadataReplacementService,
        SeasonCombinationService seasonCombinationService,
        Plugin plugin,
        PluginActivityTracker? activityTracker = null)
    {
        _libraryManager = libraryManager;
        _metadataReplacementService = metadataReplacementService;
        _seasonCombinationService = seasonCombinationService;
        _plugin = plugin;
        _activityTracker = activityTracker;
    }

    /// <summary>
    /// Replaces all metadata for a library.
    /// </summary>
    /// <param name="libraryId">The library ID.</param>
    /// <param name="replaceImages">Whether to replace images. Defaults to true.</param>
    /// <returns>No content.</returns>
    [HttpPost("ReplaceMetadata")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ReplaceMetadata([FromQuery] Guid libraryId, [FromQuery] bool replaceImages = true)
    {
        var config = _plugin.Configuration;
        
        if (!config.ReplaceAllMetadata)
        {
            return BadRequest("Metadata replacement is disabled in plugin configuration.");
        }

        try
        {
            await _metadataReplacementService.ReplaceMetadataForLibraryAsync(
                libraryId,
                replaceImages && config.ReplaceImages,
                HttpContext.RequestAborted);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error replacing metadata: {ex.Message}");
        }
    }

    /// <summary>
    /// Combines all seasons of a series into a single season.
    /// </summary>
    /// <param name="seriesId">The series ID.</param>
    /// <returns>No content.</returns>
    [HttpPost("CombineSeasons")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> CombineSeasons([FromQuery] Guid seriesId)
    {
        var config = _plugin.Configuration;
        
        if (!config.CombineAllSeasons)
        {
            return BadRequest("Season combination is disabled in plugin configuration.");
        }

        try
        {
            var series = _libraryManager.GetItemById(seriesId) as Series;
            if (series == null)
            {
                return NotFound($"Series with ID {seriesId} not found.");
            }

            await _seasonCombinationService.CombineSeasonsAsync(series, HttpContext.RequestAborted);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error combining seasons: {ex.Message}");
        }
    }

    /// <summary>
    /// Combines seasons for all series in a library.
    /// </summary>
    /// <param name="libraryId">The library ID.</param>
    /// <returns>No content.</returns>
    [HttpPost("CombineSeasonsForLibrary")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> CombineSeasonsForLibrary([FromQuery] Guid libraryId)
    {
        var config = _plugin.Configuration;
        
        if (!config.CombineAllSeasons)
        {
            return BadRequest("Season combination is disabled in plugin configuration.");
        }

        try
        {
            await _seasonCombinationService.CombineSeasonsForLibraryAsync(
                libraryId,
                HttpContext.RequestAborted);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error combining seasons: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets the plugin status and statistics.
    /// </summary>
    /// <returns>The plugin status.</returns>
    [HttpGet("PluginStatus")]
    [ProducesResponseType(typeof(PluginStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<PluginStatusResponse> GetPluginStatus()
    {
        try
        {
            var config = _plugin.Configuration;
            var stats = _activityTracker?.GetStatistics();

            var response = new PluginStatusResponse
            {
                PluginName = _plugin.Name,
                PluginVersion = _plugin.GetType().Assembly.GetName().Version?.ToString() ?? "1.0.0",
                IsEnabled = config.ReplaceAllMetadata,
                Configuration = new PluginStatusConfiguration
                {
                    ReplaceAllMetadata = config.ReplaceAllMetadata,
                    ReplaceImages = config.ReplaceImages,
                    ForceMetadataRefresh = config.ForceMetadataRefresh,
                    CombineAllSeasons = config.CombineAllSeasons
                },
                Statistics = stats != null ? new PluginStatusStatistics
                {
                    TotalItemsProcessed = stats.TotalItemsProcessed,
                    TotalItemsSucceeded = stats.TotalItemsSucceeded,
                    TotalItemsFailed = stats.TotalItemsFailed,
                    LastActivityTime = stats.LastActivityTime,
                    PluginStartTime = stats.PluginStartTime,
                    RecentActivityCount = stats.RecentActivity.Count
                } : null,
                Message = config.ReplaceAllMetadata 
                    ? "✅ Plugin is ACTIVE - All library refreshes will replace ALL metadata automatically!" 
                    : "⚠️ Plugin is DISABLED - Enable 'Replace All Metadata' in plugin settings to activate."
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new PluginStatusResponse
            {
                PluginName = "Jellyfin Easier Life",
                PluginVersion = "Unknown",
                IsEnabled = false,
                Message = $"❌ Error getting plugin status: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Gets detailed activity log.
    /// </summary>
    /// <returns>The activity log.</returns>
    [HttpGet("PluginActivity")]
    [ProducesResponseType(typeof(List<ActivityRecord>), StatusCodes.Status200OK)]
    public ActionResult<List<ActivityRecord>> GetPluginActivity()
    {
        try
        {
            var stats = _activityTracker?.GetStatistics();
            if (stats == null)
            {
                return Ok(new List<ActivityRecord>());
            }

            return Ok(stats.RecentActivity);
        }
        catch (Exception)
        {
            return Ok(new List<ActivityRecord>());
        }
    }
}

/// <summary>
/// Plugin status response.
/// </summary>
public class PluginStatusResponse
{
    public string PluginName { get; set; } = string.Empty;
    public string PluginVersion { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public PluginStatusConfiguration Configuration { get; set; } = new();
    public PluginStatusStatistics? Statistics { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Plugin configuration status.
/// </summary>
public class PluginStatusConfiguration
{
    public bool ReplaceAllMetadata { get; set; }
    public bool ReplaceImages { get; set; }
    public bool ForceMetadataRefresh { get; set; }
    public bool CombineAllSeasons { get; set; }
}

/// <summary>
/// Plugin statistics status.
/// </summary>
public class PluginStatusStatistics
{
    public int TotalItemsProcessed { get; set; }
    public int TotalItemsSucceeded { get; set; }
    public int TotalItemsFailed { get; set; }
    public DateTime? LastActivityTime { get; set; }
    public DateTime? PluginStartTime { get; set; }
    public int RecentActivityCount { get; set; }
}

