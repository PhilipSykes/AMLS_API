using Common.Constants;
using Common.Models;
using Common.Database.Interfaces;
using Common.Exceptions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;


namespace Common.Database
{
    public interface IReservationRepository
    {
        public Task<Operations.Response<bool>> CreateReservation(Entities.Reservations reservation);
        // TODO - Implement:
        //public Task<Operations.Response<bool>> ExtendReservation(?);
        //public Task<Operations.Response<bool>> CancelReservation(?);
    }

    public class ReservationRepository : IReservationRepository
    {
        private readonly IMongoDatabase _database;

        public ReservationRepository(IOptions<MongoDBConfig> options)
        {
            var config = options.Value;
            var client = new MongoClient(config.ConnectionString);
            _database = client.GetDatabase(config.DatabaseName);
        }

        public async Task<Operations.Response<bool>> CreateReservation(Entities.Reservations reservation)
        {
            
            var physical = _database.GetCollection<BsonDocument>("PhysicalMedia");
            using (var session = _database.Client.StartSession())
            {
                // Transaction settings
                var transactionOptions = new TransactionOptions(
                    readConcern: ReadConcern.Majority,   // Means search the majority of nodes, to get the most recent data, essential to prevent conflicts
                    writeConcern: WriteConcern.WMajority);  // Write ops must be acknowledged by most nodes, balance between consistency and performance

                try
                {
                    session.StartTransaction(transactionOptions);

                    // Search for specified media, but only if available, then update status
                    var physicalStatusUpdate = physical.UpdateOne(
                        session, Builders<BsonDocument>.Filter.And(
                            Builders<BsonDocument>.Filter.Eq("info", new ObjectId("673f6d28e580ac7f9f4fa9b3")), // MediaInfo Id of the item being reserved
                            //builder.Eq("branch", new ObjectID(BranchID)), TODO - Search specific branch
                            //Builders<BsonDocument>.Filter.Eq("status", "available"), // Status may be redundant now
                            Builders<BsonDocument>.Filter.Not(  // Checks for reservation collisions
                                Builders<BsonDocument>.Filter.Or(
                                    Builders<BsonDocument>.Filter.Lte("reservations.startDate", reservation.EndDate),
                                    Builders<BsonDocument>.Filter.Gte("reservations.endDate", reservation.StartDate)))
                            ),            
                        // Builders<BsonDocument>.Update.Set("status", "reserved")); //TODO - Figure out what to do with status. Redundant?
                        Builders<BsonDocument>.Update.Push("reservations", reservation));

                    // Check if it worked
                    if (physicalStatusUpdate.ModifiedCount == 0)
                    {
                        throw new InvalidOperationException("Failed to update physical media");
                    }
                    
                    session.CommitTransaction();
                    Console.WriteLine("Transaction committed successfully :)");
                }
                catch (Exception ex) // TODO? - Could catch different exceptions separately
                {
                    // If something goes wrong, abort transaction
                    session.AbortTransaction();
                    Console.WriteLine($"Transaction aborted: {ex.Message}");
                    return new Operations.Response<bool> // Note from Will: It may be better to use an enum here.
                    {
                        Success = false,
                        Message = "Operation could not be completed",
                    };
                }
            }

            return new Operations.Response<bool> // Note from Will: It may be better to use an enum here.
            {
                Success = true
            };
        }
    }
}
