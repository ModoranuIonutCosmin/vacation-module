using VacationsModule.Application.DTOs.Auth;

namespace VacationsModule.Application.Auth;

public interface IAuthenticationService
{
    // Task<RegisterUserDataModelResponse> RegisterEmployeeAsync(
    //     RegisterUserDataModelRequest registerData);

    Task<UserProfileDetailsApiModel> LoginAsync(LoginUserDataModel loginData);
}