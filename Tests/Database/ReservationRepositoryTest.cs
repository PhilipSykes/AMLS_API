using System.Diagnostics;
using Common;
using Common.Constants;
using Common.Database;
using ReservationService;
using Common.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using static Common.Models.Entities;
namespace Tests.Database;

[TestFixture]
[TestOf(typeof(ReservationRepository))]
public class ReservationRepositoryTest
{
    private IMongoDatabase _database;
    private ReservationRepository _repository;
    private string _ConnectionString = "mongodb+srv://c1023778:X4M8yMPq6DNgrOck@cluster0.simvp.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";
    private string _testDbName = "AdvancedMediaLibraryTesting";

    private string _testMember = "67236480b4d08aab049740aa";
    
    
    [OneTimeSetUp]
    public async Task Setup()
    {
        var settings = Options.Create(new MongoDBConfig
        {
            ConnectionString = _ConnectionString,
            DatabaseName = _testDbName
        });
        _repository = new ReservationRepository(settings);
        var client = new MongoClient(_ConnectionString);
        _database = client.GetDatabase(_testDbName);
        await SetupTestDatabase();
        
    }
    
    public async Task SetupTestDatabase()
    {
        await _database.CreateCollectionAsync("Reservations");
        await _database.CreateCollectionAsync("Members");
        await _database.CreateCollectionAsync("PhysicalMedia");
        await _database.CreateCollectionAsync("MediaInfo");
        await _database.CreateCollectionAsync("Branches");
        
        var members = _database.GetCollection<Members>("Members");
        var physical = _database.GetCollection<PhysicalMedia>("PhysicalMedia");
        var mediaInfo = _database.GetCollection<MediaInfo>("MediaInfo");
        var reservations = _database.GetCollection<Reservation>("Reservations");
        var branches = _database.GetCollection<Branch>("Branches");
        
        await members.InsertOneAsync(new Members
        {
            ObjectId = _testMember,
            FirstName = "Test User",
            History = []
        });
        
        
        await mediaInfo.InsertOneAsync(new MediaInfo
        {
            ObjectId = "673f6cfbe27bd6488d0ab1a7",
            Title = "Test Book"
        });
        
        
        await physical.InsertOneAsync(new PhysicalMedia
        {
            Id = "673f6d28e580ac7f9f4fa9cc",
            InfoRef = "673f6cfbe27bd6488d0ab1a7",
            Location = "67236fc4b4d08aab049740ca",
            Status = "available"
        });
        await physical.InsertOneAsync(new PhysicalMedia
        {
            Id = "673f6d28e580ac7f9f4fa9cd",
            InfoRef = "673f6cfbe27bd6488d0ab1a7",
            Location = "67236fc4b4d08aab049740ca",
            Status = "available"
        });
        await physical.InsertOneAsync(new PhysicalMedia
        {
            Id = "673f6d28e580ac7f9f4fa9ce",
            InfoRef = "673f6cfbe27bd6488d0ab1a7",
            Location = "67236fc4b4d08aab049740ca",
            Status = "available"
        });

        await branches.InsertOneAsync(new Branch
        {
            ObjectId = "67236fc4b4d08aab049740ca",
            Name = "Test Branch"
        });
        
    }
    
    [OneTimeTearDown]
    public async Task Cleanup()
    {
        await _database.DropCollectionAsync("Reservations");
        await _database.DropCollectionAsync("Members");
        await _database.DropCollectionAsync("PhysicalMedia");
        await _database.DropCollectionAsync("MediaInfo");
        await _database.DropCollectionAsync("Branches");
    }
    
    
    
    
    // TESTS //
    
    [Test]
    public async Task GetReservableItemsTest()
    {
        string media = "673f6cfbe27bd6488d0ab1a7";
        string[] branches = ["67236fc4b4d08aab049740ca", "673348f4a1ed2d02eef46a41"];
        int days = 7;


        var result = await _repository.GetReservableItems(media, branches, days);
        
        Assert.That(result.Success, Is.True);
        Debug.WriteLine($"Get Reservables Result: {result.Message}");
    }
    
    
    [Test]
    public async Task CreateReservationsTest()
    {
        
        Entities.Reservation reservation = new Entities.Reservation
        {
            ObjectId = ObjectId.GenerateNewId().ToString(),
            Item = "673f6d28e580ac7f9f4fa9cc",
            Member = _testMember,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(7),
        };
        
        var result = await _repository.CreateReservation(reservation);
        
        Assert.That(result.Success, Is.True);
        Debug.WriteLine($"Get Reservables Result: {result.Message}");
    }
    
    [Test]
    public async Task CreateReservationsConflictTest()
    {
        
        Entities.Reservation reservation1 = new Entities.Reservation
        {
            ObjectId = ObjectId.GenerateNewId().ToString(),
            Item = "673f6d28e580ac7f9f4fa9cc",
            Member = _testMember,
            StartDate = DateTime.Today.AddDays(8),
            EndDate = DateTime.Today.AddDays(12),
        };
        Entities.Reservation reservation2 = new Entities.Reservation
        {
            ObjectId = ObjectId.GenerateNewId().ToString(),
            Item = "673f6d28e580ac7f9f4fa9cc",
            Member = _testMember,
            StartDate = DateTime.Today.AddDays(9),
            EndDate = DateTime.Today.AddDays(13),
        };
        
        var result1 = await _repository.CreateReservation(reservation1);
        var result2 = await _repository.CreateReservation(reservation2);
        
        Assert.That(result1.Success, Is.True);
        Assert.That(result2.Success, Is.False);
        Debug.WriteLine($"Conflict Test Result: {result2.Message}");
    }

    [Test]
    public async Task CancelReservationsTest()
    {
        string reservationId = ObjectId.GenerateNewId().ToString();
        Entities.Reservation reservation = new Entities.Reservation
        {
            ObjectId = reservationId,
            Item = "673f6d28e580ac7f9f4fa9cc",
            Member = _testMember,
            StartDate = DateTime.Today.AddDays(20),
            EndDate = DateTime.Today.AddDays(22),
        };

        await _repository.CreateReservation(reservation);
        var result = await _repository.CancelReservation(reservationId);
        Assert.That(result.Success, Is.True);
        Debug.WriteLine($"Cancel Test Result: {result.Message}");
    }
    
    [Test]
    public async Task ExtendReservationsTest()
    {
        string reservationId = ObjectId.GenerateNewId().ToString();
        Entities.Reservation reservation = new Entities.Reservation
        {
            ObjectId = reservationId,
            Item = "673f6d28e580ac7f9f4fa9cc",
            Member = _testMember,
            StartDate = DateTime.Today.AddDays(25),
            EndDate = DateTime.Today.AddDays(27),
        };

        await _repository.CreateReservation(reservation);
        var result = await _repository.ExtendReservation(reservationId, DateTime.Today.AddDays(29));
        Assert.That(result.Success, Is.True);
        Debug.WriteLine($"Extend Test Result: {result.Message}");
    }
    
    [Test]
    public async Task CheckOutTest()
    {
        
        var result = await _repository.CheckOut("673f6d28e580ac7f9f4fa9cd", _testMember);
        Assert.That(result.Success, Is.True);
        Debug.WriteLine($"Check-out Test Result: {result.Message}");
    }
    
    [Test]
    public async Task CheckOutWithReservationTest()
    {
        Entities.Reservation reservation = new Entities.Reservation
        {
            ObjectId = ObjectId.GenerateNewId().ToString(),
            Item = "673f6d28e580ac7f9f4fa9ce",
            Member = _testMember,
            StartDate = DateTime.Today.AddDays(25),
            EndDate = DateTime.Today.AddDays(27),
        };

        await _repository.CreateReservation(reservation);
        var result = await _repository.CheckOut("673f6d28e580ac7f9f4fa9ce", _testMember);
        Assert.That(result.Success, Is.True);
        Debug.WriteLine($"Check-out Test Result: {result.Message}");
    }
    
}