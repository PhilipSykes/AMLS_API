using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.MessageBroker;
using Common.Constants;
using Common.Models;
using static Common.Models.Shared;
using static Common.Models.Operations;
using Common.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Services.UserService;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")] 
public class AuthController : ControllerBase
{
    private readonly Exchange _exchange;
    private readonly IUserSearch _userSearch;
    
    public AuthController(Exchange exchange, IUserSearch userSearch)
    {
        _exchange = exchange;
        _userSearch = userSearch;
    }

    [HttpPost("login")]
    public async Task<ActionResult<Response<AccessToken>>> Login([FromBody] Request<PayLoads.Login> request)
    {
        Console.WriteLine($"Login request for user: {request.Data.Email}");

        var emailFilter = new List<Filter>
        {
            new Filter(DbFieldNames.Login.Email, request.Data.Email, DbOperations.Equals)
        };
        List<Entities.Login> result = await _userSearch.GetLoginCredentials(emailFilter);

        if (!result.Any())
        {
            return Unauthorized(new Response<AccessToken>
            {
                Success = false,
                Message = "No Results Found",
            });
        }

        var passwordService = new PasswordService();
            //if (!passwordService.VerifyPassword(response.Data[0].PasswordHash, request.Data.Password))
            if (result[0].PasswordHash != request.Data.Password)
            {
                return Unauthorized(new Response<AccessToken>
                {
                    Success = false,
                    Message = "Invalid Credentials",
                });
            }

            // await _exchange.PublishNotification(
            //     MessageTypes.EmailNotifications.Login, 
            //     request.EmailDetails);
            string token = GenerateJwtToken(result[0]);
            Console.WriteLine($"token: {token}");
            return Ok(new Response<AccessToken>
            {
                Success = true,
                Message = "Login successful",
                Data = new AccessToken()
                {
                    Token = token
                }
            });
    }


    private string GenerateJwtToken(Entities.Login user)
    {
        var claims = new List<Claim>
        {
            // Standard claims
            new Claim(ClaimTypes.NameIdentifier, user.Username),
            new Claim(ClaimTypes.Email, user.Email),      
            new Claim(ClaimTypes.Role, user.Role), 

            // Custom claims
            new Claim("permissions", "read,write,delete")               // Specific permissions
        };
        
        string testSecretKey = "your_very_long_secret_key_min_16_chars";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(testSecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "your_issuer",
            audience: "your_audience",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public async Task HashAllPasswords() //TEMP 
    {
        var response = await _userSearch.GetLoginCredentials(null);
 
        var passwordService = new PasswordService();
        var updatedLogins = response.Select(login => new Entities.Login
        {
            ObjectID = login.ObjectID,
            Username = login.Username,
            Email = login.Email,
            PasswordHash = passwordService.HashPassword(login.PasswordHash),
            Role = login.Role
        }).ToList();

        //TODO write method needed UpdateLoginCredentials(updatedLogins);
    }
    
}