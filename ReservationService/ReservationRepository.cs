using Common.Constants;
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
        public Task<Response<List<ReservableItem>>> GetReservableItems(string media, string[] branches, int minimumLengthDays);

        public Task<Response<bool>> CheckIn(string reservationId);
        public Task<Response<bool>> CheckOut(string physicalId, string memberId, int reservationLength = 7);
    }

    
    public class ReservationRepository : IReservationRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Reservation> _reservations;
        private readonly IMongoCollection<Members> _users;
        private readonly IMongoCollection<PhysicalMedia> _physical;

        
        public ReservationRepository(IOptions<MongoDBConfig> options)
        {
            var config = options.Value;
            var client = new MongoClient(config.ConnectionString);
            _database = client.GetDatabase(config.DatabaseName);
            _reservations = _database.GetCollection<Reservation>("Reservations");
            _users = _database.GetCollection<Members>(DocumentTypes.Members);
            _physical = _database.GetCollection<PhysicalMedia>("PhysicalMedia");
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


        public async Task<Response<List<ReservableItem>>> GetReservableItems(string media, string[] branches, int minimumLengthDays)
        {
            Console.WriteLine(media, branches[0], minimumLengthDays);
            List<ReservableItem> reservables = new List<ReservableItem>();
            const string lookupField = "reservations";
            const string lookupField2 = "branchInfo";
            try
            {
                // aggregate physicalmedia with reservations
                // where info is media, and branch is in branches
                // sort reservations by start date
                // for each item, parse reservations, find time gaps bigger than minimum days
                // add to list

                var items = await _physical.Aggregate()
                    .Match(i => i.InfoRef == media && branches.Contains(i.Location))
                    .Lookup(DocumentTypes.Reservations, DbFieldNames.Id, DbFieldNames.Reservations.Item, lookupField)
                    .Lookup(DocumentTypes.Branches, DbFieldNames.PhysicalMedia.Branch, DbFieldNames.Id, lookupField2)
                    .Project(new BsonDocument
                    {
                        { "branch", "$branch" },
                        {
                            lookupField, new BsonDocument
                            {
                                {
                                    "$sortArray", new BsonDocument
                                    {
                                        { "input", "$" + lookupField },
                                        { "sortBy", new BsonDocument { { DbFieldNames.Reservations.StartDate, 1 } } }
                                    }
                                }
                            }
                        }
                    })
                    .ToListAsync();

                DateTime start = DateTime.Today;
                DateTime end;
                List<Timeslot> timeslots = new();
                // The code below finds timeslots for each item when it can be reserved.
                // It isn't perfect though, because it still searches in ascending order, and skips until it gets to present/future reservations
                // I'm keeping it this way (for now) because the start always contains the last end date by the end of the loop,
                // otherwise I have to find and store before the loop which looks a bit ugly
                foreach (var item in items)
                {
                    foreach (var reservation in item[lookupField].AsBsonArray)
                    {
                        if (reservation[DbFieldNames.Reservations.EndDate].ToUniversalTime() < DateTime.Today)
                            continue; // Skip through the reservations in the past

                        end = reservation[DbFieldNames.Reservations.StartDate].ToUniversalTime();
                        if (end > start.AddDays(minimumLengthDays))
                        {
                            timeslots.Add(new Timeslot(start, end));
                        }

                        start = reservation[DbFieldNames.Reservations.EndDate].ToUniversalTime();
                    }

                    reservables.Add(new ReservableItem
                    {
                        Item = item["_id"].ToString(),
                        BranchName = item["branch"].ToString(),
                        Timeslots = timeslots,
                        LastEnd = start
                    });
                }


            }
            catch (Exception ex)
            {
                return new Response<List<ReservableItem>>
                {
                    Success = false,
                    Message = ex.StackTrace, //"Couldn't return reservable media due to an error",
                    StatusCode = QueryResultCode.InternalServerError
                };
            }
            
            return new Response<List<ReservableItem>>
            {
                Data = reservables,
                Success = true,
                StatusCode = QueryResultCode.Ok
            };
        }


        public async Task<Response<bool>> CheckIn(string physicalId)
        {
            var t = await _physical.UpdateOneAsync(p => p.Id == physicalId,
                Builders<PhysicalMedia>.Update.Set(p => p.Status, "available"));
            if (t is null)
            {
                return new Response<bool>
                {
                    Success = false,
                    Message = "Couldn't find item",
                    StatusCode = QueryResultCode.NotFound
                };
            }

            return new Response<bool>
            {
                Success = true,
                StatusCode = QueryResultCode.Ok
            };
        }

        
        public async Task<Response<bool>> CheckOut(string physicalId, string memberId, int reservationLength = 7)
        {
            
            // If reserved by current user, allow checkout
            if (await CheckReservationOwner(physicalId, DateTime.Now) == memberId)
            {
                await _physical.UpdateOneAsync(p => p.Id == physicalId, Builders<PhysicalMedia>.Update.Set(p => p.Status, "borrowed"));
                return new Response<bool>
                {
                    Success = true,
                    StatusCode = QueryResultCode.Ok
                };
            }
            
            // Otherwise, try to create a reservation; allow checkout if no conflicts
            var resId = ObjectId.GenerateNewId().ToString();
            var start = DateTime.Today;
            var end = DateTime.Today.AddDays(reservationLength);
            var result = await CreateReservation(new Reservation
            {
                ObjectId = resId,
                Item = physicalId,
                Member = memberId,
                StartDate = start,
                EndDate = end
            });
            if (result.Success)
            {
                await _physical.UpdateOneAsync(p => p.Id == physicalId, Builders<PhysicalMedia>.Update.Set(p => p.Status, "borrowed"));
            }

            return result;
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
            //Todo - What if item not found? Change this to use Find() instead if i get time
            // Find overlapping reservations for the current item
            var result = await _reservations.CountDocumentsAsync(
                r => r.Item == item && r.ObjectId != excludeReservationId &&
                (r.EndDate >= startDate && r.StartDate <= endDate));
            
            return result == 0;
        }

        // Searches for this item at this time, to see who is currently entitled to it, if anyone.
        private async Task<string> CheckReservationOwner(string physicalid, DateTime now)
        {
            var result = await _reservations.Find(r => r.Item == physicalid && r.StartDate <= now && r.EndDate > now).FirstOrDefaultAsync();
            if (result == null)
            {
                return "";
            }

            return result.Member;
        }
    }
}