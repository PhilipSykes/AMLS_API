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
    private string _testMediaInfoId, _testMediaInfoBadId;
    private string _testMediaItemId, _testMediaWithReservationsId;
    private MediaInfo _testMediaItem = new();
    /// <summary>
    /// Current MediaInfo item state in database:
    /// <code>
    /// {
    ///     "ObjectId": "676d95357f30b6d3411432d7",
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
    ///     "ObjectId": "676d95357f30b6d3411432d7",
    ///     "Title": "Updated Test Title",
    ///     "Description": "Updated test description",
    ///     "ReleaseDate": "2024-01-01T00:00:00Z",
    ///     "Type": "Book",
    ///     "Genres": ["Fiction","New Genre"],
    ///     "Author": "Updated Test Author",
    ///     "Isbn": "12345",
    ///     "Publisher": "Updated Test Publisher",
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
        _testMediaInfoId = "676d95357f30b6d3411432d7";
        _testMediaWithReservationsId = "676d95ac7f30b6d3411432d8";
        
        //Physical Inventory IDs created in DB 
        _testMediaItemId = "676d9b857f30b6d3411432df";
        
        // Generate bad ID
        _testMediaInfoBadId = ObjectId.GenerateNewId().ToString();
        
        //Updated Media Item values
        _testMediaItem = new MediaInfo
        {   ObjectId = _testMediaInfoId,
            Title = "Updated Test Title",
            Description = "Updated test description",
            ReleaseDate = DateTime.UtcNow,
            Type = "Book",
            Genres = ["Fiction","New Genre"],
            Author = "Updated Test Author",
            Isbn = "12345",
            Publisher = " Updated Test Publisher",
            Language = "English"
        };
    }

    [Test]
    public async Task EditExistingMediaTest()
    {
        var result = await _inventoryManager.EditExistingMedia(_testMediaItem);

        Assert.That(result.Success, Is.True);
        Assert.That(result.StatusCode, Is.EqualTo(QueryResultCode.Ok));
        Debug.WriteLine($"Edit Media Result: {result.Message}");
    }

    [Test]
    public async Task EditExistingMediaBadIdTest()
    {
        _testMediaItem.ObjectId = _testMediaInfoBadId;
        var result = await _inventoryManager.EditExistingMedia(_testMediaItem);

        Assert.That(result.Success, Is.False);
        Assert.That(result.StatusCode, Is.EqualTo(QueryResultCode.NotFound));
        Debug.WriteLine($"Edit BadId Media Result: {result.Message}");
    }

    [Test]
    public async Task DeleteMediaItemTest()
    {
        // This test will only pass if the media item exists and has no reservations
        var result = await _inventoryManager.DeleteMediaItem(_testMediaItemId);
        
        Assert.That(result.Success, Is.True);
        Assert.That(result.StatusCode, Is.EqualTo(QueryResultCode.Ok));
        Debug.WriteLine($"Delete Media Result: {result.Message}");
    }

    [Test]
    public async Task DeleteMediaItemBadIdTest()
    {
        var result = await _inventoryManager.DeleteMediaItem(_testMediaInfoBadId);

        Assert.That(result.Success, Is.False);
        Assert.That(result.StatusCode, Is.EqualTo(QueryResultCode.NotFound));
        Debug.WriteLine($"Delete BadId Media Result: {result.Message}");
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