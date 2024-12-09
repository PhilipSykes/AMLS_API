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

/// <summary>
/// Controller for managing media inventory operations
/// </summary>
[ApiController]
[Route("[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryManager _inventoryManager;
    private readonly IUserSearch _userSearch;
    private readonly IMediaSearch _mediaSearch;

    /// <summary>
    /// Initializes a new instance of the InventoryController
    /// </summary>
    /// <param name="mediaSearch">Service for searching media items</param>
    /// <param name="inventoryManager">Service for managing inventory operations</param>
    /// <param name="userSearch">Service for user-related searches</param>
    public InventoryController(IMediaSearch mediaSearch,IInventoryManager inventoryManager,IUserSearch userSearch)
    {
        _inventoryManager = inventoryManager;
        _userSearch = userSearch;
        _mediaSearch = mediaSearch;
    }
    
    /// <summary>
    /// Retrieves paginated inventory data including media items and branches
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="count">Items per page</param>
    /// <returns>Response containing media info and branches list</returns>
    [Authorize(Policy = Policies.CanViewInventory)]
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
    

    /// <summary>
    /// Searches media items with specified filters and pagination
    /// </summary>
    /// <param name="filters">List of filters to apply to the search</param>
    /// <param name="page">Page number</param>
    /// <param name="count">Items per page</param>
    /// <returns>Filtered list of media items</returns>
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
    
    /// <summary>
    /// Creates a new media item in specified branch
    /// </summary>
    /// <param name="branchId">ID of the branch where media will be created</param>
    /// <param name="item">Media item details</param>
    /// <returns>Response indicating creation success</returns>
    [Authorize(Policy = Policies.CanCreateMedia)]
    [HttpPost("{branchId}/create")]
    public async Task<ActionResult<Response<string>>> Create(string branchId, [FromBody] Request<MediaInfo> item)
    {
        if (!HasBranchAccess(branchId))
        {
            return Forbid("User does not have permission to create items within this branch.");
        }
        return await _inventoryManager.CreateMedia(item);
    }
    
    /// <summary>
    /// Updates an existing media item in specified branch
    /// </summary>
    /// <param name="branchId">ID of the branch containing the media</param>
    /// <param name="item">Updated media item details</param>
    /// <returns>Response indicating update success</returns>
    [Authorize(Policy = Policies.CanEditMedia)]
    [HttpPut("{branchId}/edit")]
    public async Task<ActionResult<Response<string>>> Update(string branchId, [FromBody] Request<MediaInfo> item)
    {
        if (!HasBranchAccess(branchId))
        {
            return Forbid("User does not have permission to edit items from this branch.");
        }
        return await _inventoryManager.EditExistingMedia(item);
    }
    
    /// <summary>
    /// Deletes an existing media item from specified branch
    /// </summary>
    /// <param name="branchId">ID of the branch containing the media</param>
    /// <param name="item">Media item to delete</param>
    /// <returns>Response indicating deletion success</returns>
    [Authorize(Policy = Policies.CanDeleteMedia)]
    [HttpDelete("{branchId}/delete")]
    public async Task<ActionResult<Response<string>>> Delete(string branchId, [FromBody] Request<MediaInfo> item)
    {
        if (!HasBranchAccess(branchId))
        {
            return Forbid("User does not have permission to delete items from this branch.");
        }
        return await _inventoryManager.DeleteExistingMedia(item);
    }

    /// <summary>
    /// Checks if the current user has access to the specified branch
    /// </summary>
    /// <param name="branchId">ID of the branch to check</param>
    /// <returns>True if user has access, false otherwise</returns>
    private bool HasBranchAccess(string branchId)
    {
        var branchClaims = User.Claims
            .Where(c => c.Type == PolicyClaims.BranchAccess)
            .Select(c => c.Value);

        return branchClaims.Contains(branchId);
    }
}