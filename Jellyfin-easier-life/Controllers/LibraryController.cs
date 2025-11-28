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

    /// <summary>
    /// Initializes a new instance of the <see cref="LibraryController"/> class.
    /// </summary>
    /// <param name="libraryManager">The library manager.</param>
    /// <param name="metadataReplacementService">The metadata replacement service.</param>
    /// <param name="seasonCombinationService">The season combination service.</param>
    /// <param name="plugin">The plugin instance.</param>
    public LibraryController(
        ILibraryManager libraryManager,
        MetadataReplacementService metadataReplacementService,
        SeasonCombinationService seasonCombinationService,
        Plugin plugin)
    {
        _libraryManager = libraryManager;
        _metadataReplacementService = metadataReplacementService;
        _seasonCombinationService = seasonCombinationService;
        _plugin = plugin;
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
}

