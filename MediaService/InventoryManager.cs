using Common;
using Common.Constants;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using static Common.Models.Operations;
using static Common.Models.Entities;

namespace MediaService;

public interface IInventoryManager
{
    Task<Response<string>> EditExistingMedia(MediaInfo item);
    Task<Response<string>> DeleteMediaItem(string mediaId);
}
public class InventoryManager : IInventoryManager
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<PhysicalMedia> _physicalMedia;
    private readonly IMongoCollection<MediaInfo> _mediaInfo;
    private readonly IMongoCollection<Reservation> _reservations;

    public InventoryManager(IOptions<MongoDBConfig> options)
    {
        var config = options.Value;
        var client = new MongoClient(config.ConnectionString);
        _database = client.GetDatabase(config.DatabaseName);
        _physicalMedia = _database.GetCollection<PhysicalMedia>(DocumentTypes.PhysicalMedia);
        _mediaInfo = _database.GetCollection<MediaInfo>(DocumentTypes.MediaInfo);
        _reservations = _database.GetCollection<Reservation>(DocumentTypes.Reservations);
    }
    /// <summary>
    /// Updates an existing media item's information
    /// </summary>
    /// <param name="item">Updated media information</param>
    /// <returns>Response indicating success or failure of the update operation</returns>
    public async Task<Response<string>> EditExistingMedia(MediaInfo item)
    {
        try
        {
            using var session = await _database.Client.StartSessionAsync();
            session.StartTransaction();

            // verify the record exists
            var mediaInfo = await _mediaInfo.Find(session,
                    m => m.ObjectId == item.ObjectId)
                .FirstOrDefaultAsync();

            if (mediaInfo == null)
            {
                await session.AbortTransactionAsync();
                return new Response<string>
                {
                    Success = false,
                    Message = "Media item not found",
                    StatusCode = QueryResultCode.NotFound
                };
            }

            // Update MediaInfo collection
            var mediaInfoResult = await _mediaInfo.ReplaceOneAsync(session, 
                m => m.ObjectId == item.ObjectId,
                item);

            if (mediaInfoResult.ModifiedCount == 0)
            {
                await session.AbortTransactionAsync();
                return new Response<string>
                {
                    Success = false,
                    Message = "Failed to update document",
                    StatusCode = QueryResultCode.NotFound
                };
            }

            await session.CommitTransactionAsync();
            return new Response<string>
            {
                Success = true,
                Message = "Media updated successfully",
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
    /// Deletes a media item if it has no active reservations
    /// </summary>
    /// <param name="mediaId">ID of the media item to delete</param>
    /// <returns>Response indicating success or failure of the deletion operation</returns>
    public async Task<Response<string>> DeleteMediaItem(string mediaId)
    {
        try
        {
            using var session = await _database.Client.StartSessionAsync();
            session.StartTransaction();

            // Check for active reservations
            var hasReservations = await _reservations.Find(session,
                r => r.Item == mediaId && r.EndDate > DateTime.UtcNow
            ).AnyAsync();

            if (hasReservations)
            {
                await session.AbortTransactionAsync();
                return new Response<string>
                {
                    Success = false,
                    Message = "Cannot delete media with active reservations",
                    StatusCode = QueryResultCode.Conflict
                };
            }

            // Delete physical media
            var deleteResult = await _physicalMedia.DeleteOneAsync(session, p => p.Id == mediaId);
            if (deleteResult.DeletedCount == 0)
            {
                await session.AbortTransactionAsync();
                return new Response<string>
                {
                    Success = false,
                    Message = "Media item not found",
                    StatusCode = QueryResultCode.NotFound
                };
            }
            await session.CommitTransactionAsync();
            return new Response<string>
            {
                Success = true,
                Message = "Media deleted successfully",
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