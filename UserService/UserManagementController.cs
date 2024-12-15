using Common.Constants;
using Common.Database;
using Common.MessageBroker;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Common.Models.Operations;
using static Common.Models.Shared;
using static Common.Models.Entities;

namespace UserService;

/// <summary>
/// Controller for managing user operations
/// </summary>
[ApiController]
[Route("[controller]")]
public class UserManagementController : ControllerBase
{
    private readonly Exchange _exchange;
    private readonly ISearchRepository<Staff> _staffSearchRepo;
    private readonly ISearchRepository<Members> _memberSearchRepo;
    private readonly ISearchRepository<Branch> _branchSearchRepo;
    private readonly IUserManager _userManager;
    
    /// <summary>
    /// Initializes a new instance of the UserController
    /// </summary>
    /// <param name="exchange">Message broker exchange service</param>
    /// <param name="staffSearchRepo">Service for staff search operations</param>
    /// <param name="memberSearchRepo">Service for member search operations</param>
    /// <param name="branchSearchRepo">Service for branch search operations</param>
    /// <param name="userManager">Perform user management operations</param>
    public UserManagementController(Exchange exchange, ISearchRepository<Staff> staffSearchRepo,
        ISearchRepository<Members> memberSearchRepo,ISearchRepository<Branch> branchSearchRepo,
            IUserManager userManager)
    {
        _exchange = exchange;
        _staffSearchRepo = staffSearchRepo;
        _branchSearchRepo = branchSearchRepo;
        _memberSearchRepo = memberSearchRepo;
        _userManager = userManager;
    }
    
    [Authorize(Policy = Policies.CanViewStaff)]
    [HttpGet("staff")]
    public async Task<PaginatedResponse<PayLoads.StaffData>> GetStaff(int page, int count)
    {
        (int, int) pagination = ((page - 1) * count, count);
        var config = new AgreggateSearchConfig 
        {
            ProjectionString = $"{{ '{DbFieldNames.Staff.PhoneNumber}': 0, '{DbFieldNames.Staff.Email}': 0 }}"
        };
        
        var staffTask = _staffSearchRepo.PaginatedSearch(DocumentTypes.Staff, pagination,filters: null, config);
        
        var branchTask = _branchSearchRepo.Search(DocumentTypes.Branches);
        
        await Task.WhenAll(staffTask, branchTask);
        
        var staffList = await staffTask;
        var branches = await branchTask;
            
        if(staffList.MatchCount == 0 || branches == null)
        {
            return new PaginatedResponse<PayLoads.StaffData>
            {
                Success = false,
                StatusCode = QueryResultCode.InternalServerError,
            };
        }
            
        return new PaginatedResponse<PayLoads.StaffData>
        {
            Success = true,
            StatusCode = QueryResultCode.Ok,
            MatchCount = staffList.MatchCount,
            Data = new PayLoads.StaffData()
            {
                StaffList = staffList.Data,
                BranchesList = branches
            }
        };
    }
    
    [Authorize(Policy = Policies.CanViewMembers)]
    [HttpGet("members")]
    public async Task<PaginatedResponse<PayLoads.MemberData>> GetMembers(int page, int count)
    {
        (int, int) pagination = ((page - 1) * count, count);
        var config = new AgreggateSearchConfig 
        {
            ProjectionString = $"{{ '{DbFieldNames.Members.Settings}': 0, '{DbFieldNames.Members.Favourites}': 0, '{DbFieldNames.Members.Email}': 0 }}"
        };
        
        var memberTask = _memberSearchRepo.PaginatedSearch(DocumentTypes.Members, pagination,filters: null,config);
        
        var branchTask = _branchSearchRepo.Search(DocumentTypes.Branches);
        await Task.WhenAll(memberTask, branchTask);
        
        var memberList = await memberTask;
        var branches = await branchTask;
            
        if(memberList.MatchCount == 0 || branches == null)
        {
            return new PaginatedResponse<PayLoads.MemberData>
            {
                Success = false,
                StatusCode = QueryResultCode.InternalServerError,
            };
        }
            
        return new PaginatedResponse<PayLoads.MemberData>
        {
            Success = true,
            StatusCode = QueryResultCode.Ok,
            MatchCount = memberList.MatchCount,
            Data = new PayLoads.MemberData()
            {
                MemberList = memberList.Data,
                BranchesList = branches
            }
        };
    }
    
    [Authorize(Policy = Policies.CanViewStaff)]
    [HttpPost("staff/search")]
    public async Task<ActionResult<PaginatedResponse<List<Staff>>>> SearchStaff(List<Filter> filters, int page, int count)
    {
        (int, int) pagination = ((page - 1) * count, count);
        var config = new AgreggateSearchConfig 
        {
            ProjectionString = $"{{ '{DbFieldNames.Staff.PhoneNumber}': 0, '{DbFieldNames.Staff.Email}': 0 }}"
        };

        return await _staffSearchRepo.PaginatedSearch(DocumentTypes.Staff, pagination, filters,config);
    }
    
    [Authorize(Policy = Policies.CanViewMembers)]
    [HttpPost("members/search")]
    public async Task<ActionResult<PaginatedResponse<List<Members>>>> SearchMembers(List<Filter> filters, int page, int count)
    {
        (int, int) pagination = ((page - 1) * count, count);
        var config = new AgreggateSearchConfig 
        {
            ProjectionString = $"{{ '{DbFieldNames.Members.Settings}': 0, '{DbFieldNames.Members.Favourites}': 0, '{DbFieldNames.Members.Email}': 0 }}"
        };

        return await _memberSearchRepo.PaginatedSearch(DocumentTypes.Members, pagination, filters,config);
    }
    
    [Authorize(Policy = Policies.CanEditUserRoles)]
    [Authorize(Policy = Policies.CanEditUserPermissions)]
    [HttpPut("staff/edit")]
    public async Task<ActionResult<Response<string>>> EditStaff(Request<PayLoads.StaffUser> user)
    {
        var config = new AgreggateSearchConfig 
        {
            //TODO incorporate aggreggate for editing role in login + staff table if applicable
            ProjectionString = $"{{ '{DbFieldNames.Members.Settings}': 0, '{DbFieldNames.Members.Favourites}': 0, '{DbFieldNames.Members.Email}': 0 }}"
        };
        
        return await _userManager.EditStaff(user);
    }
    
    
    [Authorize(Policy = Policies.CanDeleteUserAccounts)]
    [HttpDelete("members/delete")]
    public async Task<ActionResult<Response<string>>> DeleteMember(Request<PayLoads.MemberUser> user)
    {
        var config = new AgreggateSearchConfig 
        {
            //TODO incorporate aggreggate for deleting from login + member table
            ProjectionString = $"{{ '{DbFieldNames.Members.Settings}': 0, '{DbFieldNames.Members.Favourites}': 0, '{DbFieldNames.Members.Email}': 0 }}"
        };
        
        return await _userManager.DeleteMember(user);
    }
    
    [Authorize(Policy = Policies.CanDeleteUserAccounts)]
    [HttpDelete("staff/delete")]
    public async Task<ActionResult<Response<string>>> DeleteStaff(Request<PayLoads.StaffUser> user)
    {
        var config = new AgreggateSearchConfig 
        {
            //TODO incorporate aggreggate for deleting from login + staff table
            ProjectionString = $"{{ '{DbFieldNames.Members.Settings}': 0, '{DbFieldNames.Members.Favourites}': 0, '{DbFieldNames.Members.Email}': 0 }}"
        };
        
        return await _userManager.DeleteStaff(user);
    }
}