using Common.Constants;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using static Common.Models.Shared;
using static Common.Models.Operations;
using static Common.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.MediaService;
using Services.UserService;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryManager _inventoryManager;
    private readonly IUserSearch _userSearch;
    private readonly IMediaSearch _mediaSearch;

    public InventoryController(IMediaSearch mediaSearch,IInventoryManager inventoryManager,IUserSearch userSearch)
    {
        _inventoryManager = inventoryManager;
        _userSearch = userSearch;
        _mediaSearch = mediaSearch;
    }
    
    [Authorize(Policy = Policies.RequireViewBranchMedia)]
    [HttpGet]
    public async Task<ActionResult<Response<PayLoads.Inventory>>> Get([FromQuery] int page, [FromQuery] int count)
    {
        (int, int) pagination = ((page - 1) * count, count);
        List<MediaInfo> mediaInfoList = await _mediaSearch.SearchMedia(pagination, filters: null);
        List<Branch> branches = await _userSearch.GetBranches(filters: null);
        return new Response<PayLoads.Inventory>
        {
            Success = true,
            Data = new PayLoads.Inventory 
            {
                MediaInfoList = mediaInfoList,
                BranchesList = branches
            }
        };

    }
    

    [HttpPost("search")]
    public async Task<ActionResult<Response<List<MediaInfo>>>> SearchMedia(List<Filter> filters, int page, int count)
    {
        (int, int) pagination = ((page - 1) * count, count);
        List<MediaInfo> mediaInfoList = await _mediaSearch.SearchMedia(pagination, filters);
        Console.WriteLine($"inventory media search successful");
        Console.WriteLine($"{mediaInfoList.Count} media found");
        return new Response<List<MediaInfo>>
        {
            Success = true,
            Data = mediaInfoList
        };
    }
    
    
    [Authorize(Policy = Policies.RequireCreateMedia)]
    [HttpPost("{branchId}/create")]
    public async Task<ActionResult<Response<string>>> Create(string branchId, [FromBody] Request<MediaInfo> item)
    {
        if (!HasBranchAccess(branchId))
        {
            return Forbid("User does not have permission to create items within this branch.");
        }
        return await _inventoryManager.CreateMedia(item);
    }
    
    [Authorize(Policy = Policies.RequireEditMedia)]
    [HttpPut("{branchId}/edit")]
    public async Task<ActionResult<Response<string>>> Update(string branchId, [FromBody] Request<MediaInfo> item)
    {
        if (!HasBranchAccess(branchId))
        {
            return Forbid("User does not have permission to edit items from this branch.");
        }
        return await _inventoryManager.EditExistingMedia(item);
    }
    
    [Authorize(Policy = Policies.RequireDeleteMedia)]
    [HttpDelete("{branchId}/delete")]
    public async Task<ActionResult<Response<string>>> Delete(string branchId, [FromBody] Request<MediaInfo> item)
    {
        if (!HasBranchAccess(branchId))
        {
            return Forbid("User does not have permission to delete items from this branch.");
        }
        return await _inventoryManager.DeleteExistingMedia(item);
    }

    private bool HasBranchAccess(string branchId)
    {
        var branchClaims = User.Claims
            .Where(c => c.Type == PolicyClaims.BranchAccess)
            .Select(c => c.Value);

        return branchClaims.Contains(branchId);
    }
}