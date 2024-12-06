using Api.MessageBroker;
using static Common.Models.Shared;
using static Common.Models.Operations;
using static Common.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.MediaService;

namespace Api.Controllers;

/// <summary>
/// Controller for managing catalog operations
/// </summary>
[ApiController]
[Route("[controller]")]
public class CatalogController : ControllerBase
{
    private readonly Exchange _exchange;
    private readonly IMediaSearch _mediaSearch;
    
    /// <summary>
    /// Initializes a new instance of the CatalogController
    /// </summary>
    /// <param name="exchange">Message broker exchange</param>
    /// <param name="mediaSearch">Service for searching media items</param>
    public CatalogController(Exchange exchange, IMediaSearch mediaSearch)
    {
        _exchange = exchange;
        _mediaSearch = mediaSearch;
    }
    
    /// <summary>
    /// Retrieves paginated media items
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="count">Items per page</param>
    /// <returns>List of media items</returns>
    [HttpGet]
    public async Task<ActionResult<Response<List<MediaInfo>>>> GetMedia(int page, int count)
    {
        (int, int) pagination = ((page - 1) * count, count);
        Console.WriteLine($"Received GET request for catalog media content");
        Console.WriteLine($"Received pagination settings: {pagination} page:{page} count:{count}");

        List<MediaInfo> mediaInfoList = await _mediaSearch.SearchMedia(pagination, filters: null);
   
        Console.WriteLine($"catalog media fetch completed successfully");
        return new Response<List<MediaInfo>>
        {
            Success = true,
            Data = mediaInfoList
        };
    }
    
    /// <summary>
    /// Searches media items with specified filters and pagination
    /// </summary>
    /// <param name="filters">List of filters to apply</param>
    /// <param name="page">Page number</param>
    /// <param name="count">Items per page</param>
    /// <returns>Filtered list of media items</returns>
    [HttpPost("search")]
    public async Task<ActionResult<Response<List<MediaInfo>>>> SearchMedia(List<Filter> filters, int page, int count)
    {
        (int, int) pagination = ((page - 1) * count, count);
        Console.WriteLine($"Received POST media search request with {filters.Count} filters");
        Console.WriteLine($"Received pagination settings: {pagination} page:{page} count:{count}");

        List<MediaInfo> mediaInfoList = await _mediaSearch.SearchMedia(pagination, filters);
        return new Response<List<MediaInfo>>
        {
            Success = true,
            Data = mediaInfoList
        };
    }
}