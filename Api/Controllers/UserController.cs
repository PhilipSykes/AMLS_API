using Api.MessageBroker;
using Microsoft.AspNetCore.Mvc;
using Services.UserService;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly Exchange _exchange;
    private readonly IUserSearch _userSearch;

    public UserController(Exchange exchange, IUserSearch userSearch)
    {
        _exchange = exchange;
        _userSearch = userSearch;
    }
}