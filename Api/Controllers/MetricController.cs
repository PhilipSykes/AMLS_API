using Api.MessageBroker;
using Common.Constants;
using Common.Models;
using static Common.Models.Shared;
using static Common.Models.Operations;
using Common.Utils;
using Microsoft.AspNetCore.Mvc;
using Services.MetricService;
using Services.UserService;
using Services.TokenAuthService;

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