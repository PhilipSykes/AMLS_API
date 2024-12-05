using Api.MessageBroker;
using Common.Constants;
using Microsoft.AspNetCore.Authorization;
using static Common.Models.Shared;
using static Common.Models.Operations;
using static Common.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.MediaService;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class InventoryController : BaseMediaController
{
    private readonly IInventoryManager _inventoryManager;

    public InventoryController(Exchange exchange, IMediaSearch mediaSearch, IInventoryManager inventoryManager)
        : base(exchange, mediaSearch)
    {
        _inventoryManager = inventoryManager;
    }
    
    //[Authorize(Policy = Policies.RequireViewBranchMedia)]
    [HttpGet]
    public async Task<ActionResult<Response<List<MediaInfo>>>> Get([FromQuery] int page, [FromQuery] int count)
    {
        
        return await GetMedia(page, count, "staff inventory");
    }
    
    [HttpPost("search")]
    public async Task<ActionResult<Response<List<MediaInfo>>>> Search(
        [FromBody] List<Filter> filters, 
        [FromQuery] int page, 
        [FromQuery] int count)
    {
        return await SearchMedia(filters, page, count);
    }
    
    [Authorize(Policy = Policies.RequireCreateMedia)]
    [HttpPost("/{branchId}/create")]
    public async Task<ActionResult<Response<string>>> Create(string branchId, [FromBody] Request<MediaInfo> item)
    {
        if (!HasBranchAccess(branchId))
        {
            return Forbid("User does not have permission to create items within this branch.");
        }
        return await _inventoryManager.CreateMedia(item);
    }
    
    [Authorize(Policy = Policies.RequireEditMedia)]
    [HttpPut("/{branchId}/edit")]
    public async Task<ActionResult<Response<string>>> Update(string branchId, [FromBody] Request<MediaInfo> item)
    {
        if (!HasBranchAccess(branchId))
        {
            return Forbid("User does not have permission to edit items from this branch.");
        }
        return await _inventoryManager.EditExistingMedia(item);
    }
    
    [Authorize(Policy = Policies.RequireDeleteMedia)]
    [HttpDelete("/{branchId}/delete")]
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
        // Admin can edit all branches
        if (User.HasClaim(c => c.Type == PolicyClaims.AdminClaim))
        {
            return true;
        }
        
        var branchClaims = User.Claims
            .Where(c => c.Type == PolicyClaims.BranchAccess)
            .Select(c => c.Value);

        return branchClaims.Contains(branchId);
    }
}