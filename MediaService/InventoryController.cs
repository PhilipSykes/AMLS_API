using Common;
using Common.Constants;
using Common.Database;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using static Common.Models.Shared;
using static Common.Models.Operations;
using static Common.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MediaService;

/// <summary>
/// Controller for managing media inventory operations
/// </summary>
[ApiController]
[Route("[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryManager _inventoryManager;
    private readonly ISearchRepository<MediaInfo> _mediaSearchRepo;
    private readonly ISearchRepository<Branch> _branchSearchRepo;

    /// <summary>
    /// Initializes a new instance of the InventoryController
    /// </summary>
    /// <param name="mediaSearchRepo">Service for searching mediainfo table</param>
    /// <param name="inventoryManager">Service for managing inventory operations</param>
    /// <param name="branchSearchRepo">Service for searching branch table</param>
    public InventoryController(ISearchRepository<MediaInfo> mediaSearchRepo,IInventoryManager inventoryManager,ISearchRepository<Branch> branchSearchRepo)
    {
        _inventoryManager = inventoryManager;
        _mediaSearchRepo = mediaSearchRepo;
        _branchSearchRepo = branchSearchRepo;
    }
    
    /// <summary>
    /// Retrieves paginated inventory data including media items and branches
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="count">Items per page</param>
    /// <returns>Response containing media info and branches list</returns>
    [Authorize(Policy = Policies.CanViewInventory)]
    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<PayLoads.Inventory>>> Get([FromQuery] int page, [FromQuery] int count)
    {
            (int, int) pagination = ((page - 1) * count, count);
        
            var mediaTask = _mediaSearchRepo.PaginatedSearch(
                DocumentTypes.MediaInfo,
                pagination,
                filters: null);
            
            var branchTask = _branchSearchRepo.Search(
                DocumentTypes.Branches,
                filters: null);

            await Task.WhenAll(mediaTask, branchTask);
        
            var mediaInfoList = await mediaTask;
            var branches = await branchTask;
            
            if(mediaInfoList.MatchCount == 0 || branches == null)
            {
                return new PaginatedResponse<PayLoads.Inventory>
                {
                    Success = false,
                    StatusCode = QueryResultCode.InternalServerError,
                };
            }
            
            return new PaginatedResponse<PayLoads.Inventory>
            {
                Success = true,
                StatusCode = QueryResultCode.Ok,
                MatchCount = mediaInfoList.MatchCount,
                Data = new PayLoads.Inventory
                {
                    MediaInfoList = mediaInfoList.Data,
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
    public async Task<ActionResult<PaginatedResponse<List<MediaInfo>>>> SearchMedia(List<Filter> filters, int page, int count)
    {
        (int, int) pagination = ((page - 1) * count, count);
        return  await _mediaSearchRepo.PaginatedSearch(DocumentTypes.MediaInfo,pagination, filters);
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