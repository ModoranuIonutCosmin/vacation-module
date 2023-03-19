namespace VacationsModule.Application.Auth;

public interface IAuthorizationService
{
    Task<bool> HasSpecificRoles(Guid userId, params string[] roles);
}