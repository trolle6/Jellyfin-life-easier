using Jellyfin.Controller;
using Jellyfin_easier_life.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin_easier_life.Controllers;

/// <summary>
/// Legacy controller route for backward compatibility.
/// </summary>
[ApiController]
[Route("Plugins/JellyfinEasierLife")]
[Authorize(Policy = "DefaultAuthorization")]
public class PluginStatusControllerLegacy : PluginStatusController
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginStatusControllerLegacy"/> class.
    /// </summary>
    /// <param name="plugin">The plugin instance.</param>
    /// <param name="activityTracker">The activity tracker.</param>
    public PluginStatusControllerLegacy(Plugin? plugin = null, PluginActivityTracker? activityTracker = null)
        : base(plugin, activityTracker)
    {
    }
}

