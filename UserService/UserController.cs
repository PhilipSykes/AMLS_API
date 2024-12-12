using Common.Constants;
using Common.Database;
using static Common.Models.Shared;
using Common.MessageBroker;
using Microsoft.AspNetCore.Mvc;
using static Common.Models.Operations;
using static Common.Models.Entities;

namespace UserService;

/// <summary>
/// Controller for managing user operations
/// </summary>
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly Exchange _exchange;
    private readonly ISearchRepository<Users> _userSearchRepo;
    
    /// <summary>
    /// Initializes a new instance of the UserController
    /// </summary>
    /// <param name="exchange">Message broker exchange service</param>
    /// <param name="userSearchRepo">Service for user search operations</param>
    public UserController(Exchange exchange, ISearchRepository<Users> userSearchRepo)
    {
        _exchange = exchange;
        _userSearchRepo = userSearchRepo;
    }
    
    public async Task<PaginatedResponse<List<Users>>> SearchUsers([FromQuery](int, int) pagination,[FromQuery] List<Filter> filters)
    {
        Console.WriteLine($"Performing user search with {filters.Count} filters");
        return await _userSearchRepo.PaginatedSearch(DocumentTypes.Users, pagination, filters);
    }
}