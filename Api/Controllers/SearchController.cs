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
        public async Task<ActionResult<SearchResponse>> Search([FromBody] List<Filter> filters)
        {
            Console.WriteLine($"Received media search request with {filters.Count} filters");
            var response = await _mediaSearchService.SearchMediaAsync(filters);
            
            if (!string.IsNullOrEmpty(response.Error))
            {
                Console.WriteLine($"Media search failed: {response.Error}");
                return StatusCode(500, response);
            }
            
            Console.WriteLine("Media search completed successfully");
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<SearchResponse>> GetInitialMedia()
        {
            Console.WriteLine("Received request for initial media content");
            var response = await _mediaSearchService.GetInitialMediaAsync();
            
            if (!string.IsNullOrEmpty(response.Error))
            {
                Console.WriteLine($"Initial media fetch failed: {response.Error}");
                return StatusCode(500, response);
            }
            
            Console.WriteLine("Initial media fetch completed successfully");
            return Ok(response);
        }
    }
