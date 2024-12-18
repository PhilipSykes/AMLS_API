using Common;
using Common.Constants;
using Common.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using static Common.Models.Operations;
using static Common.Models.PayLoads;
using static Common.Models.Entities;


namespace UserService;

public interface IUserManager
{
    Task<Response<string>> EditStaff(Request<StaffUser> user);
    Task<Response<string>> DeleteStaff(string userId);
    Task<Response<string>> DeleteMember(string userId);
}
public class UserManager : IUserManager
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<Staff> _staff;
    private readonly IMongoCollection<Members> _members;
    private readonly IMongoCollection<Entities.Login> _logins;

    public UserManager(IOptions<MongoDBConfig> options)
    {
        var config = options.Value;
        var client = new MongoClient(config.ConnectionString);
        _database = client.GetDatabase(config.DatabaseName);
        _staff = _database.GetCollection<Staff>(DocumentTypes.Staff);
        _members = _database.GetCollection<Members>(DocumentTypes.Members);
        _logins = _database.GetCollection<Entities.Login>(DocumentTypes.Login);
    }
    /// <summary>
    /// Updates an existing staff member's role and branch access
    /// </summary>
    /// <param name="request">Request containing staff user details to update</param>
    /// <returns>Response indicating success or failure of the update operation</returns>
    public async Task<Response<string>> EditStaff(Request<StaffUser> request)
    {
        try
        {
            using var session = await _database.Client.StartSessionAsync();
            session.StartTransaction();

            var user = request.Data.User;
            var result = await _staff.UpdateOneAsync(session,
                s => s.ObjectId == user.ObjectId,
                Builders<Staff>.Update
                    .Set(s => s.Role, user.Role)
                    .Set(s => s.Branches, user.Branches));

            if (result.ModifiedCount == 0)
            {
                await session.AbortTransactionAsync();
                return new Response<string>
                {
                    Success = false,
                    Message = "Staff member not found",
                    StatusCode = QueryResultCode.NotFound
                };
            }

            // Update user in login table
            await _logins.UpdateOneAsync(session, l => l.UserID == user.ObjectId,
                Builders<Entities.Login>.Update
                    .Set(l => l.Role, user.Role)
                    .Set(l => l.Branches, user.Branches));
            await session.CommitTransactionAsync();
            return new Response<string>
            {
                Success = true,
                Message = "Staff member updated successfully",
                StatusCode = QueryResultCode.Ok
            };
        }
        catch (Exception ex)
        {
            return new Response<string>
            {
                Success = false,
                Message = ex.Message,
                StatusCode = QueryResultCode.InternalServerError
            };
        }
    }
    /// <summary>
    /// Deletes a staff member and their associated login
    /// </summary>
    /// <param name="userId">ID of the staff member to delete</param>
    /// <returns>Response indicating success or failure of the deletion operation</returns>
    public async Task<Response<string>> DeleteStaff(string userId)
    {
        try
        {
            using var session = await _database.Client.StartSessionAsync();
            session.StartTransaction();

            // Delete from staff table
            var deleteStaffResult = await _staff.DeleteOneAsync(session, s => s.ObjectId == userId);

            if (deleteStaffResult.DeletedCount == 0)
            {
                await session.AbortTransactionAsync();
                return new Response<string>
                {
                    Success = false,
                    Message = "Staff member not found",
                    StatusCode = QueryResultCode.NotFound
                };
            }

            // Delete from login table
            await _logins.DeleteOneAsync(session, l => l.UserID == userId
            );

            await session.CommitTransactionAsync();
            return new Response<string>
            {
                Success = true,
                Message = "Staff member deleted successfully",
                StatusCode = QueryResultCode.Ok
            };
        }
        catch (Exception ex)
        {
            return new Response<string>
            {
                Success = false,
                Message = ex.Message,
                StatusCode = QueryResultCode.InternalServerError
            };
        }
    }
    
    /// <summary>
    /// Deletes a member and their associated login
    /// </summary>
    /// <param name="userId">ID of the member to delete</param>
    /// <returns>Response indicating success or failure of the deletion operation</returns>
    public async Task<Response<string>> DeleteMember(string userId)
    {
        try
        {
            using var session = await _database.Client.StartSessionAsync();
            session.StartTransaction();

            // Delete from members collection
            var deleteMemberResult = await _members.DeleteOneAsync(session, m => m.ObjectId == userId);
            if (deleteMemberResult.DeletedCount == 0)
            {
                await session.AbortTransactionAsync();
                return new Response<string>
                {
                    Success = false,
                    Message = "Member not found",
                    StatusCode = QueryResultCode.NotFound
                };
            }

            // Delete from login table
            await _logins.DeleteOneAsync(session, l => l.ObjectID == userId);
            await session.CommitTransactionAsync();
            return new Response<string>
            {
                Success = true,
                Message = "Member deleted successfully",
                StatusCode = QueryResultCode.Ok
            };
        }
        catch (Exception ex)
        {
            return new Response<string>
            {
                Success = false,
                Message = ex.Message,
                StatusCode = QueryResultCode.InternalServerError
            };
        }
    }
}