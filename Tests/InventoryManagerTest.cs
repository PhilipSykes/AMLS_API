using System.Diagnostics;
using Common;
using Common.Constants;
using Common.Models;
using MediaService;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using static Common.Models.Operations;
using static Common.Models.Entities;

namespace Tests;

[TestFixture]
[TestOf(typeof(InventoryManager))]
public class InventoryManagerTest
{
    private InventoryManager _inventoryManager;
    private string _testMediaId, _testMediaBadId;
    private string _testMediaWithReservationsId;
    private MediaInfo _testMediaItem,_testUpdatedMediaItem = new MediaInfo();
    
    /// <summary>
    /// Test Media States
    /// Current state in database:
    /// <code>
    /// {
    ///     "ObjectId": "673f6cfbe27bd6488d0ab1a7",
    ///     "Title": "Test Title",
    ///     "Description": "Test description",
    ///     "ReleaseDate": "2024-01-01T00:00:00Z",
    ///     "Type": "Book",
    ///     "Genres": ["Fiction"],
    ///     "Author": "Test Author",
    ///     "Isbn": "1234567890",
    ///     "Publisher": "Test Publisher",
    ///     "Language": "English"
    /// }
    /// </code>
    /// 
    /// Expected state after update:
    /// <code>
    /// {
    ///     "ObjectId": "673f6cfbe27bd6488d0ab1a7",
    ///     "Title": "Updated Test Title",
    ///     "Description": "Updated test description",
    ///     "ReleaseDate": "2024-01-01T00:00:00Z",
    ///     "Type": "Book",
    ///     "Genres": ["Fiction"],
    ///     "Author": "Test Author",
    ///     "Isbn": "1234567890",
    ///     "Publisher": "Test Publisher",
    ///     "Language": "English"
    /// }
    /// </code>
    /// </summary>
    [OneTimeSetUp]
    public void Setup()
    {
        var settings = Options.Create(new MongoDBConfig
        {
            ConnectionString = "mongodb+srv://c1023778:X4M8yMPq6DNgrOck@cluster0.simvp.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0",
            DatabaseName = "AdvancedMediaLibrary",
        });
        _inventoryManager = new InventoryManager(settings);
        
        // Test media created in DB
        _testMediaId = "673f6cfbe27bd6488d0ab1a7";
        _testMediaWithReservationsId = "673f6d28e580ac7f9f4fa9b3";
        
        // Generate bad ID
        _testMediaBadId = ObjectId.GenerateNewId().ToString();
        
        //Current Media Item values 
        _testMediaItem = new MediaInfo
        {
            ObjectId = _testMediaId,
            Title = "Test Title",
            Description = "Test description",
            ReleaseDate = DateTime.UtcNow,
            Type = "Book",
            Genres = ["Fiction"],
            Author = "Test Author",
            Isbn = "1234567890",
            Publisher = "Test Publisher",
            Language = "English"
        };
        
        //Updated Media Item values
        _testUpdatedMediaItem = new MediaInfo
        {   ObjectId = _testMediaId,
            Title = "New Test Title",
            Description = "New test description",
            ReleaseDate = DateTime.UtcNow,
            Type = "Book",
            Genres = ["Fiction","New Genre"],
            Author = "New Test Author",
            Isbn = " New 1234567890",
            Publisher = " New Test Publisher",
            Language = "English"
        };
    }

    [Test]
    public async Task EditExistingMediaTest()
    {
        var result = await _inventoryManager.EditExistingMedia(_testUpdatedMediaItem);

        Assert.That(result.Success, Is.True);
        Assert.That(result.StatusCode, Is.EqualTo(QueryResultCode.Ok));
        Debug.WriteLine($"Edit Media Result: {result.Message}");
    }

    [Test]
    public async Task EditExistingMediaBadIdTest()
    {
        _testMediaItem.ObjectId = _testMediaBadId;
        var result = await _inventoryManager.EditExistingMedia(_testMediaItem);

        Assert.That(result.Success, Is.False);
        Assert.That(result.StatusCode, Is.EqualTo(QueryResultCode.NotFound));
        Debug.WriteLine($"Edit Non-existent Media Result: {result.Message}");
    }

    [Test]
    public async Task DeleteMediaItemTest()
    {
        // This test will only pass if the media item exists and has no reservations
        var result = await _inventoryManager.DeleteMediaItem(_testMediaId);
        
        Assert.That(result.Success, Is.True);
        Assert.That(result.StatusCode, Is.EqualTo(QueryResultCode.Ok));
        Debug.WriteLine($"Delete Media Result: {result.Message}");
    }

    [Test]
    public async Task DeleteMediaItemBadIdTest()
    {
        var result = await _inventoryManager.DeleteMediaItem(_testMediaBadId);

        Assert.That(result.Success, Is.False);
        Assert.That(result.StatusCode, Is.EqualTo(QueryResultCode.NotFound));
        Debug.WriteLine($"Delete Non-existent Media Result: {result.Message}");
    }

    [Test]
    public async Task DeleteMediaItemWithReservationsTest()
    {
        var result = await _inventoryManager.DeleteMediaItem(_testMediaWithReservationsId);

        Assert.That(result.Success, Is.False);
        Assert.That(result.StatusCode, Is.EqualTo(QueryResultCode.Conflict));
        Debug.WriteLine($"Delete Media with Reservations Result: {result.Message}");
    }
}