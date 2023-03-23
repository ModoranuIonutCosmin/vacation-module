using System.Security.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VacationsModule.Application.Auth;
using VacationsModule.Application.DTOs.Auth;
using VacationsModule.Infrastructure.Seed;
using VacationsModule.WebApi.ApiResponses;
using VacationsModule.WebApi.ApiResponses.ExtensionMethods;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VacationsModule.WebApi.Controllers;

[ApiVersion("1.0")]
[ApiController]
//[ApiExplorerSettings(GroupName = "1.0")]
public class AccountController : BaseController<AccountController>
{
    private readonly IAuthenticationService _authenticationService;

    public AccountController(IAuthenticationService authenticationService,
        ILogger<AccountController> logger) : base(logger)
    {
        _authenticationService = authenticationService;
    }

    // [HttpPost("register/genericUser__sysadminonly")]
    // [Authorize(Roles = "Administrator")]
    // [ProducesResponseType(statusCode: StatusCodes.Status201Created, Type = typeof(RegisterUserDataModelResponse))]
    // [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest, Type = typeof(string))]
    // [ProducesResponseType(statusCode: StatusCodes.Status403Forbidden, Type = typeof(string))]
    // public async Task<IActionResult> RegisterAsync(
    //     [FromBody] RegisterUserDataModelRequest registerData)
    // {
    //     
    //     RegisterUserDataModelResponse response;
    //
    //     try
    //     {
    //         response = await _authenticationService.RegisterEmployeeAsync(registerData);
    //     }
    //     catch (InvalidOperationException e)
    //     {
    //         return this.Forbidden(new GenericErrorResponse()
    //         {
    //             Message = e.Message
    //         });
    //     }
    //     catch (AuthenticationException e)
    //     {
    //         return this.Forbidden(new GenericErrorResponse()
    //         {
    //             Message = e.Message
    //         });
    //     }
    //     catch (Exception e)
    //     {
    //         return BadRequest(new GenericErrorResponse()
    //         {
    //             Message = e.Message
    //         });
    //     }
    //
    //     return Created("", response);
    // }

    /// <summary>
    /// Logins with user and password. Available roles: Employee, Manager, [Administrator]
    /// </summary>
    /// <param name="loginData"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(
        [FromBody] LoginUserDataModel loginData)
    {
        UserProfileDetailsApiModel response;

        try
        {
            response = await _authenticationService.LoginAsync(loginData);
        }
        catch (AuthenticationException e)
        {
            return this.Forbidden(new GenericErrorResponse()
            {
                Message = e.Message
            });
        }

        return Accepted(response);
    }
}