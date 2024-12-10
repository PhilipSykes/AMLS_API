using Common.Database;
using MessageBroker;
using Common.Constants;
using static Common.Models.Shared;
using static Common.Models.Operations;
using static Common.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MediaService;

/// <summary>
/// Controller for managing catalog operations
/// </summary>
[ApiController]
[Route("[controller]")]
public class CatalogController : ControllerBase
{
    private readonly Exchange _exchange;
    private readonly ISearchRepository<MediaInfo> _mediaSearchRepo;
    
    /// <summary>
    /// Initializes a new instance of the CatalogController
    /// </summary>
    /// <param name="exchange">Message broker exchange</param>
    /// <param name="mediaSearchRepo">Service for searching media items</param>
    public CatalogController(Exchange exchange, ISearchRepository<MediaInfo> mediaSearchRepo)
    {
        _exchange = exchange;
        _mediaSearchRepo = mediaSearchRepo;
    }
    
    /// <summary>
    /// Retrieves paginated media items
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="count">Items per page</param>
    /// <returns>List of media items</returns>
    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<List<MediaInfo>>>> GetMedia(int page, int count)
    {
        (int, int) pagination = ((page - 1) * count, count);
        return await _mediaSearchRepo.PaginatedSearch(DocumentTypes.MediaInfo,pagination,filters: null);
    }
    
    /// <summary>
    /// Searches media items with specified filters and pagination
    /// </summary>
    /// <param name="filters">List of filters to apply</param>
    /// <param name="page">Page number</param>
    /// <param name="count">Items per page</param>
    /// <returns>Filtered list of media items</returns>
    [HttpPost("search")]
    public async Task<ActionResult<PaginatedResponse<List<MediaInfo>>>> SearchMedia(List<Filter> filters, int page, int count)
    {
        (int, int) pagination = ((page - 1) * count, count);
        return await _mediaSearchRepo.PaginatedSearch(DocumentTypes.MediaInfo,pagination,filters);
    }
}