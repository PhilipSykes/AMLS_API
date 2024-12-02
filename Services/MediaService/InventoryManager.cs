using static Common.Models.Operations;
using static Common.Models.Entities;

namespace Services.MediaService;

public interface IInventoryManager
{
    Task<Response<string>> CreateMedia(Request<MediaInfo> item);
    Task<Response<string>> EditExistingMedia(Request<MediaInfo> item);
    Task<Response<string>> DeleteExistingMedia(Request<MediaInfo> item);
}
public class InventoryManager : IInventoryManager
{

    public async Task<Response<string>> CreateMedia(Request<MediaInfo> item)
    {
        
        return new Response<string>()
        {
            Success = true,
            Message = "Item successfully added",
        };
    }
    
    public async Task<Response<string>> EditExistingMedia(Request<MediaInfo> item)
    {
        //TODO filter builder usage within to find item by ID 
        return new Response<string>()
        {
            Success = true,
            Message = "Item successfully edited",
        };
    }
    
    public async Task<Response<string>> DeleteExistingMedia(Request<MediaInfo> item)
    {
        //TODO filter builder usage within to find item by ID 
        return new Response<string>()
        {
            Success = true,
            Message = "Item successfully deleted",
        };
    }
    
    
}