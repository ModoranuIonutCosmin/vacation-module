using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using VacationsModule.Application.Auth.Config;
using VacationsModule.Domain.Entities;

namespace VacationsModule.Application.Auth.ExtensionMethods;

public class JwtService
{
    private readonly JwtOptionsProvider _jwtOptionsProvider;


    public JwtService(JwtOptionsProvider jwtOptionsProvider)
    {
        _jwtOptionsProvider = jwtOptionsProvider;
    }
    
    public string GenerateJwtToken(ApplicationUser user, IList<string> roles)
    {
        
        var claims = new[]
        {
            // Unique ID for this token
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),

            // The username using the Identity name so it fills out the HttpContext.User.Identity.Name value
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            // Add user Id so that UserManager.GetUserAsync can find the user based on Id
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        }.ToList();
        
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptionsProvider.Secret)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _jwtOptionsProvider.Issuer,
            _jwtOptionsProvider.Audience,
            claims,
            signingCredentials: credentials,
            expires: DateTime.Now.AddMonths(3)
        );


        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}