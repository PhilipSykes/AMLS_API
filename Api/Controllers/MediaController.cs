using System.Text.Json;
using Api.MessageBroker;
using Common.Constants;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Services.SearchService;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]  
public class MediaController : ControllerBase
{
    private readonly Exchange _exchange;
    private readonly IMediaSearchService _mediaSearchService;
    
    public MediaController(Exchange exchange,IMediaSearchService mediaSearchService)
    {
        _exchange = exchange;
        _mediaSearchService = mediaSearchService;
    }
    
    [HttpGet]
    public async Task<ActionResult<Operations.Response<List<Entities.MediaInfo>>>> GetInitialMedia([FromQuery] int page, [FromQuery] int count)
    {
        (int, int )pagination = ((page - 1) * count, count); 
        Console.WriteLine("Received GET request for initial media content");
        Console.WriteLine($"Received pagination settings: {pagination} page:{page} count:{count}");

        var response = await _mediaSearchService.SearchMedia(pagination,filters: null);
            
        if (!string.IsNullOrEmpty(response.Error))
        {
            Console.WriteLine($"Initial media fetch failed: {response.Error}");
            return StatusCode(500, response);
        }
            
        Console.WriteLine("Initial media fetch completed successfully");
        return Ok(response);
    }
    
    [HttpPost("search")]
    public async Task<ActionResult<Operations.Response<List<Entities.MediaInfo>>>> Search([FromBody] List<Filter> filters, [FromQuery] int page, [FromQuery] int count)
    {
        // Todo - Check url contains the pagination data. Mostly not a problem unless someone calls directly
        (int, int )pagination = ((page - 1) * count, count); 
        Console.WriteLine($"Received POST media search request with {filters.Count} filters");
        Console.WriteLine($"Received pagination settings: {pagination} page:{page} count:{count}");

        var response = await _mediaSearchService.SearchMedia(pagination, filters);
            
        if (!string.IsNullOrEmpty(response.Error))
        {
            Console.WriteLine($"Media search failed: {response.Error}");
            return StatusCode(500, response);
        }
            
        Console.WriteLine("Media search completed successfully");
        return Ok(response);
    }

    [HttpPost("reserve")]
    public async Task<ActionResult> Reserve([FromBody] ReserveRequest request)
    {
        if (request.EmailDetails.RecipientAddresses.Count == 0)
        {
            return BadRequest("Email recipients required");
        }
        
        //TODO actual reservation operation

        await _exchange.PublishNotification(
            MessageTypes.EmailNotifications.ReserveMedia, 
            request.EmailDetails);
    
        return Ok(new { message = "Reservation made" });
    }
}