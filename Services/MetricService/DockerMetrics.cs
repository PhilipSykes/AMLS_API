using System.Runtime.InteropServices;
using Docker.DotNet;
using Docker.DotNet.Models;
using System.Text.Json;



namespace Services.MetricService;

public class DockerMetrics
{
    public class ContainerStatsResponse
    {
        public CpuStats CpuStats { get; set; }
        public CpuStats PrecpuStats { get; set; }
        public MemoryStats MemoryStats { get; set; }
    }

    public class CpuStats
    {
        public CpuUsage CpuUsage { get; set; }
        public double SystemCpuUsage { get; set; }
        public int OnlineCpus { get; set; }
    }

    public class CpuUsage
    {
        public double TotalUsage { get; set; }
    }
    
    public class MemoryStats
    {
        public double Usage { get; set; }
        public double Limit { get; set; }
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
    
    private readonly DockerClient _dockerClient;

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
    
    public DockerMetrics(DockerClient dockerClient)
    {
        _dockerClient = GetDockerClient();
    }
    

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
            var stats = JsonSerializer.Deserialize<ContainerStatsResponse>(statsJson);
            
            var cpuDelta = stats.CpuStats.CpuUsage.TotalUsage - stats.PrecpuStats.CpuUsage.TotalUsage;
            var systemDelta = stats.CpuStats.SystemCpuUsage - stats.PrecpuStats.SystemCpuUsage;
            var numberOfCpus = stats.CpuStats.OnlineCpus;
            var cpuPercentage = (cpuDelta / systemDelta) * numberOfCpus * 100.0;
            
            var memoryUsageMB = stats.MemoryStats.Usage / (1024 * 1024.0);
            var memoryLimitMB = stats.MemoryStats.Limit / (1024 * 1024.0);
            var memoryPercentage = (memoryUsageMB / memoryLimitMB) * 100;
            
            metrics.Add(new Metrics
            {
                //CONTAINER
                ContainerId = container.ID,
                ContainerName = container.Names.FirstOrDefault()?.TrimStart('/'),
                //CPU
                CpuUsage = memoryUsageMB,
                DeltaCpuUsage = cpuDelta,
                CpuPercentage = cpuPercentage,
                //MEMORY
                MemoryUsage = memoryLimitMB,
                MemoryPercentage = memoryPercentage,
                MemoryLimit = memoryLimitMB,
                //TIMESTAMP
                Timestamp = DateTime.UtcNow
            });
        }
        return metrics;
    }
}