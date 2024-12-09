using Api.MessageBroker;
using Microsoft.AspNetCore.Mvc;
using Services.UserService;

namespace Api.Controllers;

/// <summary>
/// Controller for managing user operations
/// </summary>
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly Exchange _exchange;
    private readonly IUserSearch _userSearch;
    
    /// <summary>
    /// Initializes a new instance of the UserController
    /// </summary>
    /// <param name="exchange">Message broker exchange service</param>
    /// <param name="userSearch">Service for user search operations</param>
    public UserController(Exchange exchange, IUserSearch userSearch)
    {
        _exchange = exchange;
        _userSearch = userSearch;
    }
}