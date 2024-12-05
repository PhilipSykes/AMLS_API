using Api.MessageBroker;
using static Common.Models.Entities;
using static Common.Models.Operations;
using static Common.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using Services.MediaService;

namespace Api.Controllers;

public abstract class BaseMediaController : ControllerBase
{
    protected readonly Exchange _exchange;
    protected readonly IMediaSearch _mediaSearch;
    
    protected BaseMediaController(Exchange exchange, IMediaSearch mediaSearch)
    {
        _exchange = exchange;
        _mediaSearch = mediaSearch;
    }
    
    protected async Task<ActionResult<Response<List<MediaInfo>>>> GetMedia(int page, int count, string context = "")
    {
        (int, int) pagination = ((page - 1) * count, count);
        Console.WriteLine($"Received GET request for {context} media content");
        Console.WriteLine($"Received pagination settings: {pagination} page:{page} count:{count}");

        Response<List<MediaInfo>> response = await _mediaSearch.SearchMedia(pagination, filters: null);
   
        Console.WriteLine($"{context} media fetch completed successfully");
        return Ok(response);
    }
    
    protected async Task<ActionResult<Response<List<MediaInfo>>>> SearchMedia(List<Filter> filters, int page, int count)
    {
        (int, int) pagination = ((page - 1) * count, count);
        Console.WriteLine($"Received POST media search request with {filters.Count} filters");
        Console.WriteLine($"Received pagination settings: {pagination} page:{page} count:{count}");

        Response<List<MediaInfo>> response = await _mediaSearch.SearchMedia(pagination, filters);
        return Ok(response);
    }
}