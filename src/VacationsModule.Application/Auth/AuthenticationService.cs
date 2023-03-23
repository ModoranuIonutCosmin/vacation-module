using System.Security.Authentication;
using Microsoft.AspNetCore.Identity;
using VacationsModule.Application.Auth.ExtensionMethods;
using VacationsModule.Application.DTOs.Auth;
using VacationsModule.Domain.Entities;

namespace VacationsModule.Application.Auth;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtService _jwtService;

    public AuthenticationService(UserManager<ApplicationUser> userManager, JwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    // public async Task<RegisterUserDataModelResponse> RegisterEmployeeAsync(
    //     RegisterUserDataModelRequest registerData)
    // {
    //     if (registerData == null || string.IsNullOrWhiteSpace(registerData.UserName))
    //         throw new InvalidOperationException(
    //             "Bad format for request! Please provide every detail required for registration!");
    //
    //     var user = new Employee()
    //     {
    //         UserName = registerData.UserName,
    //         FirstName = registerData.FirstName,
    //         LastName = registerData.LastName,
    //         Email = registerData.Email,
    //         // Department = registerData.Department,
    //         // Position = registerData.Position
    //     };
    //
    //     var result = await _userManager.CreateAsync(user, registerData.Password);
    //
    //     if (!result.Succeeded) 
    //         throw new AuthenticationException(result.Errors.AggregateErrors());
    //     
    //     await _userManager.AddToRoleAsync(user,  RolesEnum.Employee.ToString());
    //
    //     var userIdentity = await _userManager.FindByNameAsync(user.UserName);
    //     
    //     return new RegisterUserDataModelResponse
    //     {
    //         UserName = userIdentity.UserName,
    //         FirstName = userIdentity.FirstName,
    //         LastName = userIdentity.LastName,
    //         Email = userIdentity.Email,
    //         Token = "Bearer"
    //     };
    // }



    public async Task<UserProfileDetailsApiModel> LoginAsync(LoginUserDataModel loginData)
    {
        if (loginData == null) throw new ArgumentNullException(nameof(loginData));

        //Este email?
        var isEmail = loginData.UserNameOrEmail.Contains("@");

        //Cauta userul dupa username sau email
        var user = isEmail
            ? await _userManager.FindByEmailAsync(loginData.UserNameOrEmail)
            : await _userManager.FindByNameAsync(loginData.UserNameOrEmail);


        if (user == null)
        {
            throw new AuthenticationException($"Invalid credentials [usernameOrEmail: {loginData.UserNameOrEmail}]! No user with that email or username was found!");
        }

        //Verifica daca parola este corecta fara a incrementa numarul de incercari. (peek)
        var isValidPassword = await _userManager.CheckPasswordAsync(user, loginData.Password);

        if (!isValidPassword)
        {
            throw new AuthenticationException("Invalid credentials! Invalid password!");
        }

        return new UserProfileDetailsApiModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            UserName = user.UserName,
            Token = _jwtService.GenerateJwtToken(user, await _userManager.GetRolesAsync(user)),
        };
    }
}