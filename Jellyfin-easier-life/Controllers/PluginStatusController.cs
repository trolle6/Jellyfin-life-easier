using Jellyfin.Controller;
using Jellyfin_easier_life.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin_easier_life.Controllers;

/// <summary>
/// Controller for plugin status and statistics.
/// </summary>
[ApiController]
[Route("EasierLife")]
[Authorize(Policy = "DefaultAuthorization")]
public class PluginStatusController : BaseJellyfinApiController
{
    private readonly Plugin _plugin;
    private readonly PluginActivityTracker? _activityTracker;

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginStatusController"/> class.
    /// </summary>
    /// <param name="plugin">The plugin instance.</param>
    /// <param name="activityTracker">The activity tracker (optional).</param>
    public PluginStatusController(Plugin plugin, PluginActivityTracker? activityTracker = null)
    {
        _plugin = plugin;
        _activityTracker = activityTracker;
    }

    /// <summary>
    /// Gets the plugin status and statistics.
    /// </summary>
    /// <returns>The plugin status.</returns>
    [HttpGet("Status")]
    [ProducesResponseType(typeof(PluginStatusResponse), StatusCodes.Status200OK)]
    public ActionResult<PluginStatusResponse> GetStatus()
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

    /// <summary>
    /// Gets detailed activity log.
    /// </summary>
    /// <returns>The activity log.</returns>
    [HttpGet("Activity")]
    [ProducesResponseType(typeof(List<ActivityRecord>), StatusCodes.Status200OK)]
    public ActionResult<List<ActivityRecord>> GetActivity()
    {
        var stats = _activityTracker?.GetStatistics();
        if (stats == null)
        {
            return Ok(new List<ActivityRecord>());
        }

        return Ok(stats.RecentActivity);
    }

    /// <summary>
    /// Resets the plugin statistics.
    /// </summary>
    /// <returns>No content.</returns>
    [HttpPost("ResetStatistics")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public ActionResult ResetStatistics()
    {
        _activityTracker?.Reset();
        return NoContent();
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

