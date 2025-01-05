using System.Runtime.InteropServices;
using System.Text;
using Docker.DotNet;
using Docker.DotNet.Models;
using NUnit.Framework;
using System.Text.Json;
using MetricService;
using Moq;

namespace Tests;

[TestFixture]
public class DockerMetricsTest
{
    private Mock<IDockerClient> _mockDockerClient;
    private Mock<IContainerOperations> _mockContainerOps;
    private DockerMetrics _dockerMetrics;

    [SetUp]
    public void Setup()
    {
        // Set up container operations mock
        _mockContainerOps = new Mock<IContainerOperations>();
        
        // Set up docker client mock
        _mockDockerClient = new Mock<IDockerClient>();
        _mockDockerClient.Setup(x => x.Containers).Returns(_mockContainerOps.Object);
        
        // Create DockerMetrics instance with mocked client
        _dockerMetrics = new DockerMetrics(_mockDockerClient.Object);
    }

    [Test]
    public async Task GetContainerMetrics_ValidContainer_ReturnsMetrics()
    {
        // Arrange
        var containerId = "test-container-id";
        var containerName = "test-container";
        
        var mockContainers = new List<ContainerListResponse>
        {
            new ContainerListResponse
            {
                ID = containerId,
                Names = new[] { $"/{containerName}" }
            }
        };

        var mockStats = new DockerMetrics.ContainerStatsResponse
        {
            CpuStats = new DockerMetrics.CpuStats
            {
                CpuUsage = new DockerMetrics.CpuUsage { TotalUsage = 100.0 },
                SystemCpuUsage = 1000.0,
                OnlineCpus = 4
            },
            PrecpuStats = new DockerMetrics.CpuStats
            {
                CpuUsage = new DockerMetrics.CpuUsage { TotalUsage = 50.0 },
                SystemCpuUsage = 500.0,
                OnlineCpus = 4
            },
            MemoryStats = new DockerMetrics.MemoryStats
            {
                Usage = 1024.0 * 1024.0 * 100, // 100 MB
                Limit = 1024.0 * 1024.0 * 1024.0 // 1 GB
            }
        };

        var mockStream = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(mockStats));

        _mockContainerOps
            .Setup(x => x.ListContainersAsync(
                It.IsAny<ContainersListParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockContainers);

        _mockContainerOps
            .Setup(x => x.GetContainerStatsAsync(
                containerId,
                It.IsAny<ContainerStatsParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockStream);

        // Act
        var result = await _dockerMetrics.GetContainerMetrics();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));
        
        var metrics = result.First();
        Assert.Multiple(() =>
        {
            Assert.That(metrics.ContainerId, Is.EqualTo(containerId));
            Assert.That(metrics.ContainerName, Is.EqualTo(containerName));
            Assert.That(metrics.CpuUsage, Is.GreaterThan(0));
            Assert.That(metrics.MemoryUsage, Is.GreaterThan(0));
            Assert.That(metrics.MemoryLimit, Is.EqualTo(1024)); // 1 GB in MB
        });
    }

    [Test]
    public async Task GetContainerMetrics_NullStats_ThrowsException()
    {
        // Arrange
        var containerId = "test-container-id";
        var mockContainers = new List<ContainerListResponse>
        {
            new ContainerListResponse
            {
                ID = containerId,
                Names = new[] { "/test-container" }
            }
        };

        _mockContainerOps
            .Setup(x => x.ListContainersAsync(
                It.IsAny<ContainersListParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockContainers);

        _mockContainerOps
            .Setup(x => x.GetContainerStatsAsync(
                containerId,
                It.IsAny<ContainerStatsParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemoryStream(Encoding.UTF8.GetBytes("{}"))); // Return empty JSON object

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () => 
            await _dockerMetrics.GetContainerMetrics());
        
        Assert.That(ex.Message, Is.EqualTo("Stats deserialization failed or returned null."));
    }

    [TearDown]
    public void Cleanup()
    {
        _mockDockerClient?.VerifyAll();
        _mockContainerOps?.VerifyAll();
    }
}