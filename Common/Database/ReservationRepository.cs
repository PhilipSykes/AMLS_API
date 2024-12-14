using System.Reflection.Metadata;
using Common.Constants;
using Common.Models;
using Common.Exceptions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using static Common.Models.Entities;
using static Common.Models.Operations;

namespace Common.Database
{
    public interface IReservationRepository
    {
        public Task<Response<bool>> CreateReservation(Reservation reservation);
        public Task<Response<bool>> ExtendReservation(string reservationId, DateTime newEndDate);
        public Task<Response<bool>> CancelReservation(string reservationId);
        
    }

    
    public class ReservationRepository : IReservationRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Reservation> _reservations;
        private readonly IMongoCollection<Members> _users;
        
        public ReservationRepository(IOptions<MongoDBConfig> options)
        {
            var config = options.Value;
            var client = new MongoClient(config.ConnectionString);
            _database = client.GetDatabase(config.DatabaseName);
            _reservations = _database.GetCollection<Reservation>("Reservations");
            _users = _database.GetCollection<Members>(DocumentTypes.Members);
        }

        /// <summary>
        /// Creates a new reservation
        /// </summary>
        /// <param name="reservation">Reservation id of the reservation to extend</param>
        /// <returns>Response with a Success and Status code, as well as error message if applicable</returns>
        public async Task<Response<bool>> CreateReservation(Reservation reservation)
        {
            
            using (var session = await _database.Client.StartSessionAsync())
            {
                try
                {
                    // Transaction settings
                    var transactionOptions = new TransactionOptions(
                        readConcern: ReadConcern.Majority,   // Means search the majority of nodes, to get the most recent data, essential to prevent conflicts
                        writeConcern: WriteConcern.WMajority);  // Write ops must be acknowledged by most nodes

                    session.StartTransaction(transactionOptions);
                    
                    if (await CheckAvailability(reservation.Item, reservation.StartDate, reservation.EndDate) == false)
                    {
                        await session.AbortTransactionAsync();
                        
                        return new Response<bool>
                        {
                            Success = false,
                            Message = "Reservation conflicts with others",
                            StatusCode = QueryResultCode.Conflict
                        };
                    }
                    
                    await _reservations.InsertOneAsync(session, reservation);

                    // Add reference to member history
                    await _users.UpdateOneAsync(u => u.ObjectId == reservation.Member, 
                        Builders<Members>.Update.Push(u => u.History, reservation.ObjectId));
                    //What if user not found?
                    
                    await session.CommitTransactionAsync();
                    
                
                }  catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await session.AbortTransactionAsync();
                    
                    return new Response<bool>
                    {
                        Success = false,
                        Message = ex.Message,
                        StatusCode = QueryResultCode.InternalServerError
                    };
                }
            }
            
            return new Response<bool>
            {
                Success = true,
                StatusCode = QueryResultCode.Created
            };
        }

        /// <summary>
        /// Extends an existing reservation
        /// </summary>
        /// <param name="reservationId">Reservation id of the reservation to extend</param>
        /// <param name="newEndDate">The new, requested end date</param>
        /// <returns>Response with a Success and Status code, as well as error message if applicable</returns>
        public async Task<Response<bool>> ExtendReservation(string reservationId, DateTime newEndDate)
        {
            try
            {
                var originalReservation = await _reservations.Find(o => o.ObjectId == reservationId).FirstOrDefaultAsync();
                if (originalReservation == null) throw new InvalidOperationException("Reservation could not be extended, because it wasn't found");
            
            
                if (await CheckAvailability(originalReservation.Item, originalReservation.StartDate, newEndDate, reservationId) == false)
                {
                    return new Response<bool>
                    {
                        Success = false,
                        Message = "Reservation conflicts with others",
                        StatusCode = QueryResultCode.Conflict
                    };
                }
            
                await _reservations.UpdateOneAsync(r => r.ObjectId == reservationId,
                    Builders<Reservation>.Update.Set(r => r.EndDate, newEndDate)
                );

                return new Response<bool>
                {
                    Success = true,
                    StatusCode = QueryResultCode.NoContent
                };
            }
            catch (Exception ex)
            {
                return new Response<bool>
                {
                    Success = false,
                    Message = ex.Message,
                    StatusCode = QueryResultCode.InternalServerError
                };
            }
        }
        
        /// <summary>
        /// Deletes a reservation
        /// </summary>
        /// <param name="reservationId">Reservation id of reservation being cancelled</param>
        /// <returns>Response with a Success and Status code, as well as error message if applicable</returns>
        public async Task<Response<bool>> CancelReservation(string reservationId)
        {

            try
            {
                using (var session = await _database.Client.StartSessionAsync())
                {
                    // Delete the reservation from the reservations collection
                    var result = await _reservations.FindOneAndDeleteAsync(session, u => u.ObjectId == reservationId);
                    // Delete the reference to it in the user's history
                    await _users.FindOneAndUpdateAsync(session, u => u.ObjectId == result.Member,
                        Builders<Members>.Update.Pull(DbFieldNames.Members.History, reservationId));
                
                    if (result is null)
                    {
                        await session.AbortTransactionAsync();
                        return new Response<bool>
                        {
                            Success = false,
                            Message = "Couldn't find the reservation to cancel",
                            StatusCode = QueryResultCode.NotFound
                        };
                    }
                    
                    await session.CommitTransactionAsync();

                    return new Response<bool>
                    {
                        Success = true,
                        StatusCode = QueryResultCode.NoContent
                    };
                }
            }
            catch (Exception ex)
            {
                return new Response<bool>
                {
                    Success = false,
                    Message = ex.Message,
                    StatusCode = QueryResultCode.InternalServerError
                };
            }
        }

        /// <summary>
        /// Checks if the given item is available in the given time range
        /// </summary>
        /// <param name="item">Physical id of item being tested</param>
        /// <param name="startDate">Reservation's start date and time</param>
        /// <param name="endDate">Reservation's end date and time</param>
        /// <param name="excludeReservationId">(Optional) Reservation id for avoiding finding conflicts with itself</param>
        /// <returns>True if the reservation is possible, False if it would collide with another reservation</returns>
        private async Task<bool> CheckAvailability(string item, DateTime startDate, DateTime endDate, string? excludeReservationId = null)
        {
            var filters = Builders<Reservation>.Filter;
            //Todo - What if item not found?
            // Find overlapping reservations for the current item
            var result = await _reservations.CountDocumentsAsync(
                r => r.Item == item && r.ObjectId != excludeReservationId &&
                (r.EndDate >= startDate && r.StartDate <= endDate));
            
            return result == 0;
        }
    }
}