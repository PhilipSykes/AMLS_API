using Common.Constants;
using static Common.Models.Operations;
using static Common.Models.Entities;

namespace MediaService;

public interface IInventoryManager
{
    Task<Response<string>> CreateMedia(Request<PhysicalInventory> item);
    Task<Response<string>> EditExistingMedia(Request<PhysicalInventory> item);
    Task<Response<string>> DeleteExistingMedia(Request<PhysicalInventory> item);
}
public class InventoryManager : IInventoryManager
{

    public async Task<Response<string>> CreateMedia(Request<PhysicalInventory> item)
    {
        
        return new Response<string>()
        {
            Success = true,
            Message = "Item successfully added",
            StatusCode = QueryResultCode.Created,
        };
    }
    
    public async Task<Response<string>> EditExistingMedia(Request<PhysicalInventory> item)
    {
        
        return new Response<string>()
        {
            Success = true,
            Message = "Item successfully edited",
            StatusCode = QueryResultCode.Ok,
        };
    }
    
    public async Task<Response<string>> DeleteExistingMedia(Request<PhysicalInventory> item)
    {
        //TODO filter builder usage within to find item by ID 
        return new Response<string>()
        {
            Success = true,
            Message = "Item successfully deleted",
            StatusCode = QueryResultCode.Ok,
        };
    }
    
    
}