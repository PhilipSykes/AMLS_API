using NUnit.Framework;
using Moq;
using Common.Database;
using Common;
using Common.MessageBroker;
using Common.Models;
using MediaService;
using static Common.Models.Shared;
using static Common.Models.Operations;
using static Common.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Common.Constants;
using MongoDB.Bson;
using Microsoft.Extensions.Options;

namespace Tests;

[TestFixture]
public class CatalogControllerTests
{
    private Mock<Exchange> _mockExchange;
    private Mock<ISearchRepository<MediaInfo>> _mockSearchRepo;
    private CatalogController _catalogController;
    private static readonly DateTime TestDate = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    [SetUp]
    public void Setup()
    {
        // Set up RabbitMQ config options
        var mockOptions = new Mock<IOptions<RabbitMQConfig>>();
        mockOptions.Setup(x => x.Value).Returns(new RabbitMQConfig
        {
            HostName = "test-host",
            Port = 5672,
            UserName = "guest",
            Password = "guest",
            ExchangeName = "test-exchange"
        });

        // Create Exchange with mocked options
        _mockExchange = new Mock<Exchange>(mockOptions.Object);
        _mockSearchRepo = new Mock<ISearchRepository<MediaInfo>>();
        _catalogController = new CatalogController(_mockExchange.Object, _mockSearchRepo.Object);
        _mockSearchRepo = new Mock<ISearchRepository<MediaInfo>>();
        _catalogController = new CatalogController(_mockExchange.Object, _mockSearchRepo.Object);
    }

    [Test]
    public async Task GetMedia_ValidPagination_ReturnsPaginatedResults()
    {
        // Arrange
        int page = 1;
        int count = 10;
        var expectedPagination = (0, 10);
        
        var mediaList = new List<MediaInfo>
        {
            new() { 
                ObjectId = ObjectId.GenerateNewId().ToString(),
                Title = "Test Media 1",
                Language = "English",
                Description = "Test Description 1",
                ReleaseDate = TestDate,
                Type = "Book",
                PhysicalCopies = new List<PhysicalCopy>
                {
                    new() { 
                        Branch = ObjectId.GenerateNewId().ToString(),
                        Status = "Available"
                    }
                }
            },
            new() { 
                ObjectId = ObjectId.GenerateNewId().ToString(),
                Title = "Test Media 2",
                Language = "English",
                Description = "Test Description 2",
                ReleaseDate = TestDate,
                Type = "Book",
                PhysicalCopies = new List<PhysicalCopy>
                {
                    new() { 
                        Branch = ObjectId.GenerateNewId().ToString(),
                        Status = "Available"
                    }
                }
            }
        };

        var expectedResponse = new PaginatedResponse<List<MediaInfo>>
        {
            Data = mediaList,
            Success = true,
            Message = "Success",
            StatusCode = QueryResultCode.Ok,
            MatchCount = 2
        };

        _mockSearchRepo
            .Setup(x => x.PaginatedSearch(
                DocumentTypes.MediaInfo,
                expectedPagination,
                null,
                It.IsAny<AgreggateSearchConfig>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var actionResult = await _catalogController.GetMedia(page, count);

        // Assert
        Assert.That(actionResult.Value, Is.Not.Null);
        var value = actionResult.Value;
        
        Assert.Multiple(() =>
        {
            Assert.That(value.Data.Count, Is.EqualTo(2));
            Assert.That(value.Success, Is.True);
            Assert.That(value.StatusCode, Is.EqualTo(QueryResultCode.Ok));
            Assert.That(value.MatchCount, Is.EqualTo(2));
            Assert.That(value.Data[0].Title, Is.EqualTo("Test Media 1"));
            Assert.That(value.Data[0].PhysicalCopies, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public async Task SearchMedia_WithFilters_ReturnsPaginatedFilteredResults()
    {
        // Arrange
        int page = 1;
        int count = 10;
        var expectedPagination = (0, 10);
        
        var filters = new List<Filter>
        {
            new(
                key: DbFieldNames.MediaInfo.Title,
                value: "Test",
                operation: DbEnums.Contains
            )
        };

        var mediaList = new List<MediaInfo>
        {
            new() { 
                ObjectId = ObjectId.GenerateNewId().ToString(),
                Title = "Test Media 1",
                Language = "English",
                Description = "Test Description",
                ReleaseDate = TestDate,
                Type = "Book",
                PhysicalCopies = new List<PhysicalCopy>
                {
                    new() { 
                        Branch = ObjectId.GenerateNewId().ToString(),
                        Status = "Available"
                    }
                }
            }
        };

        var expectedResponse = new PaginatedResponse<List<MediaInfo>>
        {
            Data = mediaList,
            Success = true,
            Message = "Success",
            StatusCode = QueryResultCode.Ok,
            MatchCount = 1
        };

        _mockSearchRepo
            .Setup(x => x.PaginatedSearch(
                DocumentTypes.MediaInfo,
                expectedPagination,
                filters,
                It.IsAny<AgreggateSearchConfig>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var actionResult = await _catalogController.SearchMedia(filters, page, count);

        // Assert
        Assert.That(actionResult.Value, Is.Not.Null);
        var value = actionResult.Value;
        
        Assert.Multiple(() =>
        {
            Assert.That(value.Data.Count, Is.EqualTo(1));
            Assert.That(value.Success, Is.True);
            Assert.That(value.StatusCode, Is.EqualTo(QueryResultCode.Ok));
            Assert.That(value.MatchCount, Is.EqualTo(1));
            Assert.That(value.Data[0].Title, Is.EqualTo("Test Media 1"));
            Assert.That(value.Data[0].PhysicalCopies, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public async Task GetMedia_InvalidPage_ReturnsEmptyResults()
    {
        // Arrange
        int page = 999;
        int count = 10;
        var expectedPagination = (9980, 10);
        
        var expectedResponse = new PaginatedResponse<List<MediaInfo>>
        {
            Data = new List<MediaInfo>(),
            Success = true,
            Message = "No results found",
            StatusCode = QueryResultCode.Ok,
            MatchCount = 0
        };

        _mockSearchRepo
            .Setup(x => x.PaginatedSearch(
                DocumentTypes.MediaInfo,
                expectedPagination,
                null,
                It.IsAny<AgreggateSearchConfig>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var actionResult = await _catalogController.GetMedia(page, count);

        // Assert
        Assert.That(actionResult.Value, Is.Not.Null);
        var value = actionResult.Value;
        
        Assert.Multiple(() =>
        {
            Assert.That(value.Data.Count, Is.EqualTo(0));
            Assert.That(value.Success, Is.True);
            Assert.That(value.StatusCode, Is.EqualTo(QueryResultCode.Ok));
            Assert.That(value.MatchCount, Is.EqualTo(0));
        });
    }

    [TearDown]
    public void Cleanup()
    {
        _mockSearchRepo.VerifyAll();
    }
}