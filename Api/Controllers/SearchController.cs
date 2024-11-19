using Microsoft.AspNetCore.Mvc;
using Common.Models;
using Services.SearchService;

namespace Api.Controllers;

    [ApiController]
    [Route("[controller]")]
    public class MediaSearchController : ControllerBase 
    {
        private readonly IMediaSearchService _mediaSearchService;

        public MediaSearchController(IMediaSearchService mediaSearchService)
        {
            _mediaSearchService = mediaSearchService;
        }

        [HttpPost]
        public async Task<ActionResult<SearchResponse>> Search([FromBody] List<Filter> filters, [FromQuery] int page, [FromQuery] int count)
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

        [HttpGet]
        public async Task<ActionResult<SearchResponse>> GetInitialMedia([FromQuery] int page, [FromQuery] int count)
        {
            (int, int )pagination = ((page - 1) * count, count); 
            Console.WriteLine("Received GET request for initial media content");
            Console.WriteLine($"Received pagination settings: {pagination} page:{page} count:{count}");

            var response = await _mediaSearchService.GetInitialMedia(pagination);
            
            if (!string.IsNullOrEmpty(response.Error))
            {
                Console.WriteLine($"Initial media fetch failed: {response.Error}");
                return StatusCode(500, response);
            }
            
            Console.WriteLine("Initial media fetch completed successfully");
            return Ok(response);
        }
    }
