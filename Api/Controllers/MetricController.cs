using Microsoft.AspNetCore.Mvc;
using Services.MetricService;


namespace Api.Controllers;

[ApiController]
[Route("admin/[controller]")] 

public class MetricController : ControllerBase
{
    [HttpGet]
    public async Task<List<DockerMetrics.Metrics>> GetSnapshot()
    { 
        DockerMetrics _metricsService = new();
        List<DockerMetrics.Metrics> _snapshot = new();

        _snapshot = await _metricsService.GetContainerMetrics();
        
        return _snapshot;
    }
}