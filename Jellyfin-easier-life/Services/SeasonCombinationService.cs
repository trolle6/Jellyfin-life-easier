using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using Microsoft.Extensions.Logging;

namespace Jellyfin_easier_life.Services;

/// <summary>
/// Service for combining all seasons of a series into a single season.
/// </summary>
public class SeasonCombinationService
{
    private readonly ILogger<SeasonCombinationService> _logger;
    private readonly ILibraryManager _libraryManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="SeasonCombinationService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="libraryManager">The library manager.</param>
    public SeasonCombinationService(
        ILogger<SeasonCombinationService> logger,
        ILibraryManager libraryManager)
    {
        _logger = logger;
        _libraryManager = libraryManager;
    }

    /// <summary>
    /// Combines all seasons of a series into a single season.
    /// </summary>
    /// <param name="series">The series to combine seasons for.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task CombineSeasonsAsync(
        Series series,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Combining seasons for series: {SeriesName} ({SeriesId})", series.Name, series.Id);

            var seasons = series.GetSeasons().OrderBy(s => s.IndexNumber ?? 0).ToList();
            
            if (seasons.Count <= 1)
            {
                _logger.LogInformation("Series {SeriesName} has only one season, no combination needed", series.Name);
                return;
            }

            // Find or create the target season (use season 1 as the base)
            var targetSeason = seasons.FirstOrDefault(s => s.IndexNumber == 1) 
                ?? seasons.First();

            _logger.LogInformation("Using season {SeasonNumber} as target season", targetSeason.IndexNumber);

            // Move all episodes from other seasons to the target season
            foreach (var season in seasons)
            {
                if (season.Id == targetSeason.Id)
                {
                    continue;
                }

                var episodes = season.GetEpisodes().ToList();
                _logger.LogInformation("Moving {Count} episodes from season {SeasonNumber} to season 1", 
                    episodes.Count, season.IndexNumber);

                foreach (var episode in episodes)
                {
                    // Update episode to belong to target season
                    episode.SeasonId = targetSeason.Id;
                    episode.ParentId = targetSeason.Id;
                    
                    // Adjust episode number to account for episodes from previous seasons
                    var previousEpisodesCount = seasons
                        .Where(s => s.IndexNumber < season.IndexNumber)
                        .Sum(s => s.GetEpisodes().Count());
                    
                    if (episode.IndexNumber.HasValue)
                    {
                        episode.IndexNumber = episode.IndexNumber.Value + previousEpisodesCount;
                    }

                    await _libraryManager.UpdateItemAsync(episode, episode.GetParent(), ItemUpdateType.MetadataEdit, cancellationToken);
                }

                // Optionally remove the now-empty season
                // Note: This is a destructive operation, so we'll log it but not do it by default
                _logger.LogInformation("Season {SeasonNumber} has been emptied. Consider removing it manually if desired.", season.IndexNumber);
            }

            // Refresh the target season
            await _libraryManager.UpdateItemAsync(targetSeason, series, ItemUpdateType.MetadataEdit, cancellationToken);

            _logger.LogInformation("Successfully combined seasons for series: {SeriesName}", series.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error combining seasons for series: {SeriesName} ({SeriesId})", series.Name, series.Id);
            throw;
        }
    }

    /// <summary>
    /// Combines seasons for all series in a library.
    /// </summary>
    /// <param name="libraryId">The library ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task CombineSeasonsForLibraryAsync(
        Guid libraryId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting season combination for library: {LibraryId}", libraryId);

            var rootFolder = _libraryManager.GetItemById(libraryId);
            if (rootFolder == null)
            {
                _logger.LogWarning("Library not found: {LibraryId}", libraryId);
                return;
            }

            var series = rootFolder.GetRecursiveChildren()
                .OfType<Series>()
                .ToList();

            _logger.LogInformation("Found {Count} series to process", series.Count);

            foreach (var s in series)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Season combination cancelled");
                    break;
                }

                await CombineSeasonsAsync(s, cancellationToken);
            }

            _logger.LogInformation("Completed season combination for library: {LibraryId}", libraryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error combining seasons for library: {LibraryId}", libraryId);
            throw;
        }
    }
}

