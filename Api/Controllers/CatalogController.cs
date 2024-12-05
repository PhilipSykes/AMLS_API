using Api.MessageBroker;
using static Common.Models.Shared;
using static Common.Models.Operations;
using static Common.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.MediaService;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CatalogController : BaseMediaController
{
    public CatalogController(Exchange exchange, IMediaSearch mediaSearch) 
        : base(exchange, mediaSearch) { }
    
    [HttpGet]
    public async Task<ActionResult<Response<List<MediaInfo>>>> Get([FromQuery] int page, [FromQuery] int count)
    {
        return await GetMedia(page, count, "public catalog");
    }
    
    [HttpPost("search")]
    public async Task<ActionResult<Response<List<MediaInfo>>>> Search([FromBody] List<Filter> filters, [FromQuery] int page, [FromQuery] int count)
    {
        return await SearchMedia(filters, page, count);
    }
}