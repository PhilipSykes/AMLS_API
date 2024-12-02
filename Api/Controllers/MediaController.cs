using Api.MessageBroker;
using Common.Constants;
using Microsoft.AspNetCore.Mvc;
using Services.MediaService;
using static Common.Models.Shared;
using static Common.Models.Operations;
using static Common.Models.Entities;
using static Common.Models.PayLoads;


namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MediaController : ControllerBase
{
    private readonly Exchange _exchange;
    private readonly IMediaSearch _mediaSearch;

    public MediaController(Exchange exchange, IMediaSearch mediaSearch)
    {
        _exchange = exchange;
        _mediaSearch = mediaSearch;
    }

    [HttpGet]
    public async Task<ActionResult<Response<List<MediaInfo>>>> GetInitialMedia([FromQuery] int page,
        [FromQuery] int count)
    {
        (int, int ) pagination = ((page - 1) * count, count);
        Console.WriteLine("Received GET request for initial media content");
        Console.WriteLine($"Received pagination settings: {pagination} page:{page} count:{count}");

        var response = await _mediaSearch.SearchMedia(pagination, null);

        Console.WriteLine("Initial media fetch completed successfully");
        return Ok(response);
    }

    [HttpPost("search")]
    public async Task<ActionResult<Response<List<MediaInfo>>>> Search([FromBody] List<Filter> filters,
        [FromQuery] int page, [FromQuery] int count)
    {
        // Todo - Check url contains the pagination data. Mostly not a problem unless someone calls directly
        (int, int ) pagination = ((page - 1) * count, count);
        Console.WriteLine($"Received POST media search request with {filters.Count} filters");
        Console.WriteLine($"Received pagination settings: {pagination} page:{page} count:{count}");

        var response = await _mediaSearch.SearchMedia(pagination, filters);

        Console.WriteLine("Media search completed successfully");
        return Ok(response);
    }

    [HttpPost("reserve")]
    public async Task<ActionResult> Reserve([FromBody] Request<Reserve> request)
    {
        if (request.EmailDetails.RecipientAddresses.Count == 0) return BadRequest("Email recipients required");

        //TODO actual reservation operation: create reservation in reservations table, flag physicalmedia as reserved

        await _exchange.PublishNotification(
            MessageTypes.EmailNotifications.ReserveMedia,
            request.EmailDetails);

        return Ok(new { message = "Reservation made" });
    }
}