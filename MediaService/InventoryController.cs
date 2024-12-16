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
    private readonly ISearchRepository<PhysicalInventory> _inventorySearchRepo;
    private readonly ISearchRepository<Branch> _branchSearchRepo;
    private AgreggateSearchConfig _config = new()
    { 
        UseAggregation = true,
        LookupCollections = new List<string> 
        { 
            DocumentTypes.MediaInfo,
            DocumentTypes.Branches,
            DocumentTypes.Reservations 
        },
        LocalFields = new List<string> 
        { 
            DbFieldNames.PhysicalMedia.Info,
            DbFieldNames.PhysicalMedia.Branch,
            DbFieldNames.PhysicalMedia.Info 
        },
        ForeignFields = new List<string> 
        { 
            DbFieldNames.Id,
            DbFieldNames.Id,
            DbFieldNames.Reservations.Item 
        },
        OutputFields = new List<string> 
        { 
            DbFieldNames.Aggregates.MediaInfo,
            DbFieldNames.Aggregates.BranchDetails,
            DbFieldNames.Aggregates.Reservations 
        },
        ProjectionString = $@"{{
            '{DbFieldNames.Aggregates.MediaInfo}': {{ $arrayElemAt: ['${DbFieldNames.Aggregates.MediaInfo}', 0] }},
            '{DbFieldNames.Aggregates.BranchDetails}': {{ $arrayElemAt: ['${DbFieldNames.Aggregates.BranchDetails}', 0] }},
            '{DbFieldNames.PhysicalMedia.Status}': 1,
            '{DbFieldNames.PhysicalMedia.Branch}': 1}}"
    };

    /// <summary>
    /// Initializes a new instance of the InventoryController
    /// </summary>
    /// <param name="inventorySearchRepo">Service for searching and aggregating several tables to produce media inventory</param>
    /// <param name="inventoryManager">Service for managing inventory operations</param>
    /// <param name="branchSearchRepo">Service for searching branch table</param>
    public InventoryController(ISearchRepository<PhysicalInventory>  inventorySearchRepo,IInventoryManager inventoryManager,ISearchRepository<Branch> branchSearchRepo)
    {
        _inventoryManager = inventoryManager;
        _inventorySearchRepo =  inventorySearchRepo;
        _branchSearchRepo = branchSearchRepo;
    }
    
    /// <summary>
    /// Retrieves paginated inventory data including media items and branches
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="count">Items per page</param>
    /// <returns>Response containing media inventory and branches list</returns>
    [Authorize(Policy = Policies.CanViewInventory)]
    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<PayLoads.Inventory>>> Get([FromQuery] int page, [FromQuery] int count)
    {
            (int, int) pagination = ((page - 1) * count, count);
            
            var inventoryTask =  _inventorySearchRepo.PaginatedSearch(DocumentTypes.PhysicalMedia, pagination,filters: null,_config);
            
            var branchTask = _branchSearchRepo.Search(DocumentTypes.Branches);

            await Task.WhenAll(inventoryTask, branchTask);
        
            var mediaInventoryList = await inventoryTask;
            var branches = await branchTask;
            
            if(mediaInventoryList.MatchCount == 0 || branches == null)
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
                MatchCount = mediaInventoryList.MatchCount,
                Data = new PayLoads.Inventory
                {
                    PhysicalMediaList = mediaInventoryList.Data,
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
    /// <returns>Filtered inventory list of media items</returns>
    [HttpPost("search")]
    public async Task<ActionResult<PaginatedResponse<List<PhysicalInventory>>>> SearchMedia(List<Filter> filters, int page, int count)
    {
        (int, int) pagination = ((page - 1) * count, count);
        return await _inventorySearchRepo.PaginatedSearch(DocumentTypes.PhysicalMedia, pagination, filters,_config);
    }
    
    /// <summary>
    /// Updates an existing media item in specified branch
    /// </summary>
    /// <param name="branchId">ID of the branch containing the media</param>
    /// <param name="item">Updated media item details</param>
    /// <returns>Response indicating update success</returns>
    [Authorize(Policy = Policies.CanEditMedia)]
    [HttpPut("{branchId}/edit")]
    public async Task<ActionResult<Response<string>>> Update(string branchId, [FromBody] MediaInfo mediaInfo)
    {
        if (!HasBranchAccess(branchId))
        {
            return Forbid("User does not have permission to edit items from this branch.");
        }
        return await _inventoryManager.EditExistingMedia(mediaInfo);
    }
    
    /// <summary>
    /// Deletes an existing media item from specified branch
    /// </summary>
    /// <param name="branchId">ID of the branch containing the media</param>
    /// <param name="mediaId">ID of the Media item to delete</param>
    /// <returns>Response indicating deletion success</returns>
    [Authorize(Policy = Policies.CanDeleteMedia)]
    [HttpDelete("{branchId}/delete/{mediaId}")]
    public async Task<ActionResult<Response<string>>>Delete(string branchId, string mediaId)
    {
        if (!HasBranchAccess(branchId))
        {
            return Forbid("User does not have permission to delete items from this branch.");
        }
        return await _inventoryManager.DeleteMediaItem(mediaId);
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