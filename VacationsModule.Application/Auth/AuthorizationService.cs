using Microsoft.AspNetCore.Identity;
using VacationsModule.Domain.Entities;

namespace VacationsModule.Application.Auth;

public class AuthorizationService : IAuthorizationService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthorizationService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task<bool> HasSpecificRoles(Guid userId, params string[] roles)
    {
        
        ApplicationUser user = await _userManager.FindByIdAsync(userId.ToString());
        
        if (user == null) return false;
        
        var userRoles = await _userManager.GetRolesAsync(user);
        
        return roles.All(userRoles.Contains);
    }
}