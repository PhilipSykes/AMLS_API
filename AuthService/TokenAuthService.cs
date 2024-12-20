using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common;
using Common.Constants;
using Common.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService;


public class TokenAuthService(IOptions<JWTTokenConfig> options)
{
    private readonly JWTTokenConfig _config = options.Value;
    /// <summary>
    /// Generate a JWT token with users claims embedded
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public string GenerateJwtToken(Entities.Login user)
    {

        List<Claim> claims = AddClaims(user);
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config.Issuer,
            audience: _config.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_config.ExpirationMinutes),
            notBefore: DateTime.Now,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    /// <summary>
    /// Refresh JWT token when its close to expiry
    /// </summary>
    /// <param name="existingToken"></param>
    /// <returns></returns>
    public string RefreshToken(string existingToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(existingToken);

        var token = new JwtSecurityToken(
            issuer: _config.Issuer,
            audience: _config.Audience,
            claims: jwtToken.Claims,                   
            expires: DateTime.Now.AddMinutes(_config.ExpirationMinutes),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SecretKey)), 
                SecurityAlgorithms.HmacSha256)
        );

        return tokenHandler.WriteToken(token);
    }
    /// <summary>
    /// Adds claims to the various users
    /// </summary>
    /// <param name="user">login credentials</param>
    /// <returns></returns>
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
            case PolicyRoles.BranchLibrarian:
                claims.Add(new Claim(PolicyClaims.BranchAccess,user.Branches[0]));
                break;

            case PolicyRoles.BranchManager:
                foreach (string branch in user.Branches)
                { 
                    claims.Add(new Claim(PolicyClaims.BranchAccess,branch));
                }
                break;
            case PolicyRoles.Member:
                foreach (string branch in user.Branches)
                {
                    claims.Add(new Claim(PolicyClaims.BranchAccess,branch));
                }
                break;
        }

        return claims;
    }
    
}