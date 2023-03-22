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
public class VacationRequestsController : BaseController<VacationRequestsController>
{
    
    public VacationRequestsController(ILogger<VacationRequestsController> logger,
        UsersSeed usersSeed) : base(logger)
    {
        //SEED la USERS
        usersSeed?.Seed().Wait();
    }

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


    
    
    [Authorize(Roles="Employee")]
    [HttpGet("national-holidays")]
    [ProducesResponseType(typeof(List<NationalDay>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetNationalHolidays([FromServices] INationalDaysService nationalDaysService)
    {
        var nationalHolidays = nationalDaysService.GetNationalDays(default, default);

        return Ok(nationalHolidays);
    }


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