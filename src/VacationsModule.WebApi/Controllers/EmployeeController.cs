using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VacationsModule.Application.DTOs;
using VacationsModule.Application.DTOs.Auth;
using VacationsModule.Application.Interfaces.Services;
using VacationsModule.Domain.Exceptions;
using VacationsModule.WebApi.ApiResponses;
using VacationsModule.WebApi.ApiResponses.ExtensionMethods;

namespace VacationsModule.WebApi.Controllers;



[ApiVersion("1.0")]
[ApiController]
//[ApiExplorerSettings(GroupName = "1.0")]
public class EmployeeController : BaseController<EmployeeController>
{
    private readonly IEmployeesService _employeesService;

    public EmployeeController(ILogger<EmployeeController> logger, 
        IEmployeesService employeesService) : base(logger)
    {
        _employeesService = employeesService;
    }
    
    /// <summary>
    /// Registers a new user. Requires a manager role (one default is user:pass manageruser01:string123, use the login route to get JWT auth).
    /// </summary>
    /// <param name="registerData"></param>
    /// <returns></returns>
    [HttpPost("register/employee")]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(statusCode: StatusCodes.Status201Created, Type = typeof(RegisterUserDataModelResponse))]
    [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status403Forbidden, Type = typeof(string))]
    public async Task<IActionResult> RegisterEmployeeAsync(
        [FromBody] RegisterEmployeeDataModelRequest registerData)
    {
        
        RegisterUserDataModelResponse response;

        try
        {
            response = await _employeesService.RegisterEmployeeAsync(registerData);
        }
        catch (InvalidOperationException e)
        {
            return this.Forbidden(new GenericErrorResponse()
            {
                Message = e.Message
            });
        }
        catch (AuthenticationException e)
        {
            return this.Forbidden(new GenericErrorResponse()
            {
                Message = e.Message
            });
        }
        catch (Exception e)
        {
            return BadRequest(new GenericErrorResponse()
            {
                Message = e.Message
            });
        }

        return Created("", response);
    }

    /// <summary>
    /// Calculates the amount of left vacation days for a year.
    /// A manager is also an employee but with more privilege.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="employeesService"></param>
    /// <returns></returns>
    [HttpGet("available-vacation-days")]
    [Authorize(Roles="Manager, Employee")]
    [ProducesResponseType(typeof(GetAvailableVacationDaysResponse), 200)]
    [ProducesResponseType(typeof(GenericErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAvailableVacationDays([FromQuery] GetAvailableVacationDaysRequest request,
        [FromServices] IEmployeesService employeesService)
    {

        Guid requestingUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        GetAvailableVacationDaysResponse response;

        try
        {
            response = await employeesService.GetAvailableVacationDays(requestingUserId, request);
        }
        catch (NoVacationDaysLogsException e)
        {
            return BadRequest(new GenericErrorResponse()
            {
                Message = e.Message
            });
        }
        catch (Exception e)
        {
            return BadRequest(new GenericErrorResponse()
            {
                Message = "One or more errors occurred while counting the vacation days."
            });
        }

        return Ok(response);
    }
}