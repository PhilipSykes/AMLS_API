using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.Models;
using Microsoft.IdentityModel.Tokens;

namespace Services.TokenAuthService;

public class TokenAuthService
{
    public string GenerateJwtToken(Entities.Login user)
    {
        var claims = new List<Claim>
        {
            // Standard claims
            new(ClaimTypes.NameIdentifier, user.UserId),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role),

            // Custom claims
            new("permissions", "read,write,delete") // Specific permissions
        };

        var testSecretKey = "your_very_long_secret_key_min_16_chars";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(testSecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            "your_issuer",
            "your_audience",
            claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool ValidateToken(string token)
    {
        //TODO Add token validation logic
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                //validation parameters
            };
            handler.ValidateToken(token, tokenValidationParameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }
}