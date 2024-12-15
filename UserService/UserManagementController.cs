using Common.Constants;
using Common.Database;
using Common.MessageBroker;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Common.Models.Operations;
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
    
    /// <summary>
    /// Initializes a new instance of the UserController
    /// </summary>
    /// <param name="exchange">Message broker exchange service</param>
    /// <param name="staffSearchRepo">Service for staff search operations</param>
    /// <param name="memberSearchRepo">Service for member search operations</param>
    /// <param name="branchSearchRepo">Service for branch search operations</param>
    public UserManagementController(Exchange exchange, ISearchRepository<Staff> staffSearchRepo,
        ISearchRepository<Members> memberSearchRepo,ISearchRepository<Branch> branchSearchRepo)
    {
        _exchange = exchange;
        _staffSearchRepo = staffSearchRepo;
        _branchSearchRepo = branchSearchRepo;
        _memberSearchRepo = memberSearchRepo;
    }
    
    [Authorize(Policy = Policies.CanViewUsers)]
    [HttpGet("staff")]
    public async Task<PaginatedResponse<PayLoads.StaffData>> GetStaff(int page, int count)
    {
        (int, int) pagination = ((page - 1) * count, count);
        var config = new Shared.AgreggateSearchConfig 
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
    
    [Authorize(Policy = Policies.CanViewUsers)]
    [HttpGet("members")]
    public async Task<PaginatedResponse<PayLoads.MemberData>> GetMembers(int page, int count)
    {
        (int, int) pagination = ((page - 1) * count, count);
        var config = new Shared.AgreggateSearchConfig 
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
}