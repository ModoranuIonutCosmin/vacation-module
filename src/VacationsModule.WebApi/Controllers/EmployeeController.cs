using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VacationsModule.Application.DTOs;
using VacationsModule.Application.DTOs.Auth;
using VacationsModule.Application.Features;
using VacationsModule.Application.Interfaces.Services;
using VacationsModule.Domain.Exceptions;
using VacationsModule.WebApi.ApiResponses;
using VacationsModule.WebApi.ApiResponses.ExtensionMethods;

namespace VacationsModule.WebApi.Controllers;



[ApiVersion("1.0")]
public class EmployeeController : BaseController<EmployeeController>
{
    private readonly IEmployeesService _employeesService;

    public EmployeeController(ILogger<EmployeeController> logger, 
        IEmployeesService employeesService) : base(logger)
    {
        _employeesService = employeesService;
    }
    
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
    
    [Authorize(Roles="Employee")]
    [HttpGet("available-vacation-days")]
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
        catch (VacationRequestValidationException e)
        {
            return BadRequest(new GenericErrorResponse()
            {
                Issues = e.Issues,
                Message = "One or more errors occurred while validating the vacation request."
            });
        }

        return Ok(response);
    }
}