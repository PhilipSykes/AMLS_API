using Common.Database;
using Common.Constants;
using Common.MessageBroker;
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
    private AgreggateSearchConfig _config = new()
    {
        UseAggregation = true,
        LookupCollections = new List<string> { DocumentTypes.PhysicalMedia },
        LocalFields = new List<string> { DbFieldNames.Id },
        ForeignFields = new List<string> { DbFieldNames.PhysicalCopies.Info },
        OutputFields = new List<string> { DbFieldNames.MediaInfo.PhysicalCopies },
        ProjectionString = $@"{{  
            '{DbFieldNames.MediaInfo.PhysicalCopies}.{DbFieldNames.Id}': 0, 
            '{DbFieldNames.MediaInfo.PhysicalCopies}.{DbFieldNames.PhysicalCopies.Info}': 0 }}"
    };
    
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
        return await _mediaSearchRepo.PaginatedSearch(DocumentTypes.MediaInfo,pagination,filters: null,_config);
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
        return await _mediaSearchRepo.PaginatedSearch(DocumentTypes.MediaInfo,pagination,filters,_config);
    }
}