using Api.MessageBroker;
using static Common.Models.Shared;
using static Common.Models.Operations;
using static Common.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.MediaService;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CatalogController : ControllerBase
{
    private readonly Exchange _exchange;
    private readonly IMediaSearch _mediaSearch;
    
    public CatalogController(Exchange exchange, IMediaSearch mediaSearch)
    {
        _exchange = exchange;
        _mediaSearch = mediaSearch;
    }
    
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