using System.Text.Json;
using Api.MessageBroker;
using Common.Constants;
using Common.Database;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;


[ApiController]
[Route("[controller]")]  
public class SearchController : ControllerBase
{
    private readonly Exchange _exchange;
    
    public SearchController(Exchange exchange)
    {
        _exchange = exchange;
    }

    [HttpPost]
    public async Task<ActionResult> Search([FromBody] List<Filter> requests)
    {
        var searchMessage = requests.Select(filter => new
        {
            key = filter.key,
            value = filter.value,
            operation = filter.operation
        }).ToList();

        await _exchange.PublishSearch(
            MessageTypes.Media.Search, 
            JsonSerializer.Serialize(searchMessage));
        
        return Ok(new { message = "Searching..." });
    }
}