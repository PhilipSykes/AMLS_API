using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common;
using Common.Constants;
using Common.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Services.TokenAuthService;


public class TokenAuthService(IOptions<JWTTokenConfig> options)
{
    private readonly JWTTokenConfig _config = options.Value;
    
    public string GenerateJwtToken(Entities.Login user)
    {

        List<Claim> claims = AddClaims(user);
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config.Issuer,
            audience: _config.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
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

    private List<Claim> AddClaims(Entities.Login user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserID),
            new Claim(ClaimTypes.Email, user.Email),      
            new Claim(ClaimTypes.Role, user.Role), 
        };

        switch (user.Role)
        {
            case "Member":
                claims.Add(new Claim(PolicyClaims.MemberClaim, "true"));
                break;
                
            case "BranchLibrarian":
                claims.Add(new Claim(PolicyClaims.LibrarianClaim, "true"));
                claims.Add(new Claim(PolicyClaims.StaffAccess, "true"));
                claims.Add(new Claim(PolicyClaims.ViewBranchMedia, "true"));
                claims.Add(new Claim(PolicyClaims.BranchAccess,user.Branches[0]));
                claims.Add(new Claim(PolicyClaims.EditMedia, "true"));
                break;

            case "BranchManager":
                claims.Add(new Claim(PolicyClaims.ManagerClaim, "true"));
                claims.Add(new Claim(PolicyClaims.StaffAccess, "true"));
                    foreach (string branch in user.Branches)
                    {
                    claims.Add(new Claim(PolicyClaims.BranchAccess,branch));
                    }
                claims.Add(new Claim(PolicyClaims.ViewBranchMedia, "true"));
                claims.Add(new Claim(PolicyClaims.CreateMedia, "true"));
                claims.Add(new Claim(PolicyClaims.EditMedia, "true"));
                claims.Add(new Claim(PolicyClaims.DeleteMedia, "true"));
                claims.Add(new Claim(PolicyClaims.ManageUsers, "true"));
                claims.Add(new Claim(PolicyClaims.ViewStaffReports, "true"));
                break;
                
            case "Accountant":
                claims.Add(new Claim(PolicyClaims.AccountantClaim, "true"));
                claims.Add(new Claim(PolicyClaims.ViewFinancialReports, "true"));
                break;
                
            case "SystemAdmin": 
                claims.Add(new Claim(PolicyClaims.AdminClaim, "true"));
                claims.Add(new Claim(PolicyClaims.ManageUsers, "true"));
                claims.Add(new Claim(PolicyClaims.ViewMetricsReports, "true"));
                break;
        }

        return claims;
    }
    
}