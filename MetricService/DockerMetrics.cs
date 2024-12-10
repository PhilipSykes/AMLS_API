using System.Runtime.InteropServices;
using Docker.DotNet;
using Docker.DotNet.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MetricService;


/// <summary>
/// A service for retrieving Docker container metrics such as CPU and memory usage.
/// </summary>
public class DockerMetrics
{
    public class ContainerStatsResponse
    {
        [JsonPropertyName("cpu_stats")]
        public CpuStats CpuStats { get; set; }

        [JsonPropertyName("precpu_stats")]
        public CpuStats PrecpuStats { get; set; }

        [JsonPropertyName("memory_stats")]
        public MemoryStats MemoryStats { get; set; }
    }

    public class CpuStats
    {
        [JsonPropertyName("cpu_usage")]
        public CpuUsage CpuUsage { get; set; }

        [JsonPropertyName("system_cpu_usage")]
        public double? SystemCpuUsage { get; set; }

        [JsonPropertyName("online_cpus")]
        public int? OnlineCpus { get; set; }
    }

    public class CpuUsage
    {
        [JsonPropertyName("total_usage")]
        public double? TotalUsage { get; set; }
    }

    public class MemoryStats
    {
        [JsonPropertyName("usage")]
        public double? Usage { get; set; }

        [JsonPropertyName("limit")]
        public double? Limit { get; set; }
    }


    public class Metrics
    {
        //CONTAINER
        public string ContainerId { get; set; }
        public string ContainerName { get; set; }
        //CPU
        public double CpuUsage { get; set; }
        public double DeltaCpuUsage { get; set; }
        public double CpuPercentage { get; set; }
        
        //MEMORY
        public double MemoryUsage { get; set; }
        public double MemoryPercentage { get; set; }
        public double MemoryLimit { get; set; }
        //TIMESTAMP
        public DateTime Timestamp { get; set; }
    }
    
    public DockerClient _dockerClient { get; }

    /// <summary>
    /// Retrieves the appropriate Docker API URI for the current operating system.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> representing the Docker API URI. 
    /// - For Windows: <c>npipe://./pipe/docker_engine</c>.
    /// - For Linux or macOS: <c>unix:///var/run/docker.sock</c>.
    /// </returns>
    /// <exception cref="PlatformNotSupportedException">
    /// Thrown if the operating system is not Windows, Linux, or macOS.
    /// </exception>
    private static string GetDockerApiUri()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return "npipe://./pipe/docker_engine";
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return "unix:///var/run/docker.sock";
        }
        
        throw new PlatformNotSupportedException("This OS is not supported.");
    }

    /// <summary>
    /// Initializes and returns a Docker client configured to connect to the Docker daemon.
    /// </summary>
    /// <returns>A <see cref="DockerClient"/> instance for communicating with Docker.</returns>
    /// <exception cref="Exception">
    /// Thrown if the Docker daemon is not running or cannot be accessed due to missing permissions or configuration issues.
    /// </exception>
    public static DockerClient GetDockerClient()
    {
        try
        {
            var DockerApiUri = GetDockerApiUri();
            var config = new DockerClientConfiguration(new Uri(DockerApiUri));
            return config.CreateClient();
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException || ex is System.IO.FileNotFoundException)
        {
            throw new Exception("Cannot connect to Docker Daemon, is it running?");
        }
    }
    
    public DockerMetrics()
    {
        _dockerClient = GetDockerClient();
    }
    

    /// <summary>
    /// Retrieves metrics (CPU and memory usage) for all Docker containers.
    /// </summary>
    /// <returns>
    /// A <see cref="List{T}"/> of <see cref="Metrics"/> objects containing container statistics.
    /// </returns>
    /// <exception cref="Exception">
    /// Thrown if the Docker API response is invalid or if deserialization fails.
    /// </exception>
    public async Task<List<Metrics>> GetContainerMetrics()
    {
        var metrics = new List<Metrics>();
        var containers = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters() { All = true });
        foreach (var container in containers)
        {
            var response = await _dockerClient.Containers.GetContainerStatsAsync(
                container.ID,
                new ContainerStatsParameters{Stream = false},
                CancellationToken.None);
            
            using var reader = new StreamReader(response);
            var statsJson = await reader.ReadToEndAsync();
            
            Console.WriteLine($"Raw stats JSON for container {container.ID}: {statsJson}");
            
            var stats = JsonSerializer.Deserialize<ContainerStatsResponse>(statsJson);
            
            
            if (stats == null)
            {
                throw new Exception("Stats deserialization failed or returned null.");
            }

            if (stats.CpuStats == null || stats.CpuStats.CpuUsage == null)
            {
                throw new Exception("CpuStats or CpuUsage is null. Check Docker API response.");
            }

            if (stats.PrecpuStats == null || stats.PrecpuStats.CpuUsage == null)
            {
                throw new Exception("PrecpuStats or CpuUsage is null. Check Docker API response.");
            }
            
            var cpuDelta = (stats.CpuStats.CpuUsage?.TotalUsage ?? 0) - (stats.PrecpuStats.CpuUsage?.TotalUsage ?? 0);
            var systemDelta = (stats.CpuStats.SystemCpuUsage ?? 0) - (stats.PrecpuStats.SystemCpuUsage ?? 0);
            var numberOfCpus = stats.CpuStats.OnlineCpus ?? 1; 

            var cpuPercentage = (systemDelta > 0) 
                ? (cpuDelta / systemDelta) * numberOfCpus * 100.0 
                : 0; 

            var memoryUsageMB = (stats.MemoryStats.Usage ?? 0) / (1024 * 1024.0);
            var memoryLimitMB = (stats.MemoryStats.Limit ?? 0) / (1024 * 1024.0);
            var memoryPercentage = (memoryLimitMB > 0) 
                ? (memoryUsageMB / memoryLimitMB) * 100 
                : 0;

            metrics.Add(new Metrics
            {
                // CONTAINER
                ContainerId = container.ID,
                ContainerName = container.Names.FirstOrDefault()?.TrimStart('/'),

                // CPU
                CpuUsage = cpuDelta,
                DeltaCpuUsage = cpuDelta,
                CpuPercentage = cpuPercentage,

                // MEMORY
                MemoryUsage = memoryUsageMB,
                MemoryPercentage = memoryPercentage,
                MemoryLimit = memoryLimitMB,

                // TIMESTAMP
                Timestamp = DateTime.UtcNow
            });
        }
        return metrics;
    }
}