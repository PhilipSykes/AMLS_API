using Microsoft.AspNetCore.Mvc;

namespace MetricService;

/// <summary>
/// Controller for retrieving Docker container metrics
/// </summary>
[ApiController]
[Route("[controller]")] 
public class MetricController : ControllerBase
{
    /// <summary>
    /// Gets a snapshot of current Docker container metrics
    /// </summary>
    /// <returns>List of container metrics including CPU, memory, and container details</returns>
    [HttpGet]
    public async Task<List<DockerMetrics.Metrics>> GetSnapshot()
    { 
        DockerMetrics _metricsService = new();
        List<DockerMetrics.Metrics> _snapshot = new();

        _snapshot = await _metricsService.GetContainerMetrics();
        
        return _snapshot;
    }
}