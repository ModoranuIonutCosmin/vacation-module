using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VacationsModule.Application.DTOs;
using VacationsModule.Application.Features;
using VacationsModule.Application.Interfaces.Services;
using VacationsModule.Domain.DomainServices.Interfaces;
using VacationsModule.Domain.Exceptions;
using VacationsModule.Domain.Models;
using VacationsModule.Infrastructure.Seed;
using VacationsModule.WebApi.ApiResponses;
using VacationsModule.WebApi.ApiResponses.ExtensionMethods;

namespace VacationsModule.WebApi.Controllers;

[ApiVersion("1.0")]
[ApiController]
//[ApiExplorerSettings(GroupName = "1.0")]
public class VacationRequestsController : BaseController<VacationRequestsController>
{
    
    public VacationRequestsController(ILogger<VacationRequestsController> logger,
        UsersSeed usersSeed) : base(logger)
    {
        //SEED la USERS
        usersSeed?.Seed().Wait();
    }

    /// <summary>
    /// Gets a list of vacations request by status and possibly by employeeId
    /// Manager can access any request while the employee can only browse (filter) his requests.
    /// Status value = 1 means pending requests (requests that were just created).
    /// </summary>
    /// <param name="request"></param>
    /// <param name="vacationRequestsService"></param>
    /// <returns></returns>
    [Authorize(Roles="Manager, Employee")]
    [HttpGet("vacation-requests")]
    [ProducesResponseType(typeof(GetVacationRequestsPaginatedResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetVacationRequests([FromQuery] GetVacationRequestsPaginatedRequest request,
        [FromServices] IVacationRequestsService vacationRequestsService)
    {
        Guid currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        
        var response =
            await vacationRequestsService.GetVacationRequestsByStatusAndEmployeeId(requestingUserId: currentUserId, request);

        return Ok(response);
    }

    /// <summary>
    /// Creates a new vacation request.
    /// You can provide multiple intervals. They are validated against overlapping / being in past/ etc
    /// Example: [[21.02.2022-22.02.2022], [25.02.2022-26.02.2022]] = 4 days
    /// Dates must have the DateTimeOffset format.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="vacationRequestsService"></param>
    /// <returns></returns>
    [Authorize(Roles="Employee")]
    [HttpPost("create-vacation-request")]
    [ProducesResponseType(typeof(CreateVacationRequestResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(GenericErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateVacationRequest([FromBody] CreateVacationRequestRequest request,
        [FromServices] IVacationRequestsService vacationRequestsService)
    {
        Guid requestingUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);


        CreateVacationRequestResponse response;
        try
        {
            response = await vacationRequestsService.CreateVacationRequest(requestingUserId, request);
        }
        catch (VacationRequestValidationException e)
        {
            return BadRequest(new GenericErrorResponse()
            {
                Issues = e.Issues,
                Message = "One or more errors occurred while validating the vacation request."
            });
        }

        return Created("vacation-requests", response);
    }

    /// <summary>
    /// Updates a vacation request. Allows adding a comment to the vacation request ticket.
    /// Validates the same way as for the vacation request creation.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="vacationRequestsService"></param>
    /// <returns></returns>
    [Authorize(Roles="Employee")]
    [HttpPut("update-vacation-request")]
    [ProducesResponseType(typeof(UpdateVacationRequestResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(GenericErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateVacationRequest([FromBody] UpdateVacationRequestRequest request,
        [FromServices] IVacationRequestsService vacationRequestsService)
    {
        //TODO: Auth
        Guid requestingUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        UpdateVacationRequestResponse response;

        try
        {
            response = await vacationRequestsService.UpdateVacationRequest(requestingUserId, request);
        }
        catch (VacationRequestValidationException e)
        {
            return BadRequest(new GenericErrorResponse()
            {
                Issues = e.Issues,
                Message = "One or more errors occurred while validating the vacation request."
            });
        }
        catch (UnauthorizedAccessException e)
        {
            return new ObjectResult(new GenericErrorResponse()
            {
                Issues = new List<string>() { "You are not authorized to perform this action." },
                Message = e.Message
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        return Ok(response);
    }



    /// <summary>
    /// Gets possible national holidays within a date interval.
    /// Example: StartDate: 2023-01-22T23:14:44.080Z EndDate: 2023-03-21T23:14:44.080Z
    /// Defaults to last year.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="nationalDaysService"></param>
    /// <returns></returns>
    [Authorize(Roles="Employee")]
    [HttpGet("national-holidays")]
    [ProducesResponseType(typeof(List<NationalDay>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetNationalHolidays([FromQuery] GetNationalDaysRequest request,
    [FromServices] INationalDaysService nationalDaysService)
    {
        var nationalHolidays = nationalDaysService.GetNationalDays(request.StartDate, request.EndDate);

        return Ok(nationalHolidays);
    }

    /// <summary>
    /// Gets any request by id (if called by manager) or a specific request owned by the calling employee by id.
    /// Status value = 1 means pending requests (requests that were just created)
    /// </summary>
    /// <param name="vacationRequestId"></param>
    /// <param name="vacationRequestsService"></param>
    /// <returns></returns>
    [HttpGet("vacation-requests/{vacationRequestId}")]
    [Authorize(Roles="Manager, Employee")]
    [ProducesResponseType(typeof(VacationRequestModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetVacationRequestById([FromRoute] Guid vacationRequestId,
        [FromServices] IVacationRequestsService vacationRequestsService)
    {
        Guid requestingUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        VacationRequestModel result;

        try
        {
            result = await vacationRequestsService.GetVacationRequestById(requestingUserId, vacationRequestId);
        }

        catch (UnauthorizedAccessException e)
        {
            return this.Forbidden(new GenericErrorResponse()
            {
                Message = e.Message
            });
        }

        return result switch
        {
            null => NotFound(),
            var vacationRequest => Ok(vacationRequest)
        };
    }
    
}