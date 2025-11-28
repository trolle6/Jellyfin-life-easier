using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin_easier_life;

/// <summary>
/// Main plugin class for Jellyfin Easier Life.
/// </summary>
public class Plugin : BasePlugin<PluginConfiguration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin"/> class.
    /// </summary>
    /// <param name="applicationPaths">The application paths.</param>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public Plugin(
        MediaBrowser.Common.ApplicationPaths.IApplicationPaths applicationPaths,
        System.Net.Http.IHttpClientFactory httpClientFactory,
        Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
        : base(applicationPaths, httpClientFactory, loggerFactory)
    {
        Instance = this;
    }

    /// <summary>
    /// Gets the plugin instance.
    /// </summary>
    public static Plugin? Instance { get; private set; }

    /// <inheritdoc />
    public override string Name => "Jellyfin Easier Life";

    /// <inheritdoc />
    public override Guid Id => new Guid("A1B2C3D4-E5F6-7890-ABCD-EF1234567890");

    /// <inheritdoc />
    public override string Description => "Enhances library scanning to replace all metadata and provides universal settings like combining seasons.";

    /// <inheritdoc />
    public override PluginInfo GetPluginInfo()
    {
        return new PluginInfo
        {
            Name = Name,
            Description = Description,
            Id = Id.ToString(),
            Version = GetType().Assembly.GetName().Version?.ToString() ?? "1.0.0"
        };
    }
}

