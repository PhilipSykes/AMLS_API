using System.Diagnostics;
using Common;
using Common.Database;
using Common.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace Tests.Database;

[TestFixture]
[TestOf(typeof(ReservationRepository))]
public class ReservationRepositoryTest
{

    [Test]
    public async Task GetReservableItemsTest()
    {

        var settings = Options.Create(new MongoDBConfig
        {
            ConnectionString =
                "mongodb+srv://c1023778:X4M8yMPq6DNgrOck@cluster0.simvp.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0",
            DatabaseName = "AdvancedMediaLibrary",
        });
        var repo = new ReservationRepository(settings);
        string media = "673f6cfae27bd6488d0ab1a0";
        string[] branches = ["67236fc4b4d08aab049740ca", "673348f4a1ed2d02eef46a41"];
        int days = 7;


        var results = await repo.GetReservableItems(media, branches, days);
        Debug.WriteLine(results);
    }

    [Test]
    public async Task CreateReservationsTest()
    {
        var settings = Options.Create(new MongoDBConfig
        {
            ConnectionString =
                "mongodb+srv://c1023778:X4M8yMPq6DNgrOck@cluster0.simvp.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0",
            DatabaseName = "AdvancedMediaLibrary",
        });
        var repo = new ReservationRepository(settings);
        
        Entities.Reservation res3 = new Entities.Reservation
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Item = "673f6d28e580ac7f9f4fa9b3",
            Member = "67236480b4d08aab049740aa",
            StartDate = DateTime.Today.AddDays(60),
            EndDate = DateTime.Today.AddDays(65),
        };
        
        await repo.CreateReservation(res3);
    }
    
    
}