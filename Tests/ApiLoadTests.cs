using System.Diagnostics;


namespace Tests;

[TestFixture]
public class ApiLoadTests
{
    private HttpClient _client;
    private const int ConcurrentRequests = 100;

    [OneTimeSetUp]
    public void Setup()
    {
        
        _client = new HttpClient();
        _client.BaseAddress = new Uri("http://localhost:7000");
    }

    [Test]
    public async Task GetCatalogStressTest()
    {
        // Arrange
        var stopwatch = new Stopwatch();
        var tasks = new List<Task<HttpResponseMessage>>();
        var responseTimesMs = new List<long>();

        // Act
        stopwatch.Start();
        for (int i = 0; i < ConcurrentRequests; i++)
        {
            tasks.Add(_client.GetAsync("/api/catalog?page=1&count=10"));
        }
        var responses = await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Collect response times
        foreach (var response in responses)
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
        }

        // Assert
        var totalTime = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Total time: {totalTime}ms");
        Console.WriteLine($"Average time per request: {totalTime / ConcurrentRequests}ms");
        Assert.That(totalTime / ConcurrentRequests, Is.LessThan(500), "Average response time exceeds 500ms");
    }
    
    [OneTimeTearDown]
    public void TearDown()
    {
        _client.Dispose();
    }
}