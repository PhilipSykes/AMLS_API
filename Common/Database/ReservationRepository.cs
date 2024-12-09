using Common.Constants;
using Common.Models;
using Common.Exceptions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using static Common.Models.Entities;

namespace Common.Database
{
    public interface IReservationRepository
    {
        public Task<Operations.Response<bool>> CreateReservation(Reservation reservation);
        // TODO - Implement:
        //public Task<bool> ExtendReservation(?); // Create new reservation, Use the check availability(), if true, extend?
        //Or maybe mod CheckAvaiability to include a smaller func?
        public Task<bool> CancelReservation(ObjectId reservationId, ObjectId userId);
        
    }

    public class ReservationRepository : IReservationRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Reservation> _reservations;
        private readonly IMongoCollection<Users> _users;
        
        public ReservationRepository(IOptions<MongoDBConfig> options)
        {
            var config = options.Value;
            var client = new MongoClient(config.ConnectionString);
            _database = client.GetDatabase(config.DatabaseName);
            _reservations = _database.GetCollection<Reservation>("Reservations");
            _users = _database.GetCollection<Users>("Users");
        }

        public async Task<Operations.Response<bool>> CreateReservation(Reservation reservation)
        {

            try
            {
                using (var session = _database.Client.StartSession())
                {
                    // Transaction settings
                    var transactionOptions = new TransactionOptions(
                        readConcern: ReadConcern.Majority,   // Means search the majority of nodes, to get the most recent data, essential to prevent conflicts
                        writeConcern: WriteConcern.WMajority);  // Write ops must be acknowledged by most nodes

                    if (await CheckAvailability(reservation))
                    {
                        await _reservations.InsertOneAsync(session, reservation);

                        var filters = Builders<Users>.Filter;
                        var update = Builders<Users>.Update;
                        // Add reference to member history
                        await _users.FindOneAndUpdateAsync(
                            filters.Eq(u => u.Id, reservation.Member), 
                            update.Push(u => u.History, reservation.ObjectID));
                    }
                    
                    session.CommitTransaction();
                
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            

            return new Operations.Response<bool> // Note from Will: It may be better to use an enum here.
            {
                Success = true,
                StatusCode = QueryResultCode.Created
            };
        }


        public async Task<bool> CancelReservation(ObjectId reservationId, ObjectId userId)
        {

            try
            {
                using (var session = await _database.Client.StartSessionAsync())
                {
                    // Delete the reservation from the reservations collection
                    var result = await _reservations.DeleteOneAsync(session, 
                        Builders<Reservation>.Filter.Eq(u => u.ObjectID, reservationId));
                    // Delete the reference to it in the user's history
                    await _users.FindOneAndUpdateAsync(session, 
                        Builders<Users>.Filter.Eq(u => u.Id, userId),
                        Builders<Users>.Update.Pull(DbFieldNames.Users.History, reservationId));
                
                    await session.CommitTransactionAsync();
                    
                    return result.DeletedCount == 1;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<bool> CheckAvailability(Reservation reservation)
        {
            var filters = Builders<Reservation>.Filter;
            
            // This filter means: find overlapping reservations for the current item
            var filter = filters.And(
                filters.Eq(r => r.Item, reservation.Item),
                filters.And(
                    filters.Gte(r => r.EndDate, reservation.StartDate),
                    filters.Lte(r => r.StartDate, reservation.EndDate)));
            
            var result = await _reservations.CountDocumentsAsync(filter);
            
            return result == 0;
        }
    }
}
