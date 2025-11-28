using Jellyfin_easier_life.Hooks;
using Jellyfin_easier_life.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin_easier_life;

/// <summary>
/// Service registration for the plugin.
/// </summary>
public class PluginServiceRegistration : IPluginServiceRegistration
{
    /// <inheritdoc />
    public void RegisterServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<MetadataReplacementService>();
        serviceCollection.AddScoped<SeasonCombinationService>();
        serviceCollection.AddSingleton<LibraryScanHook>();
    }
}

