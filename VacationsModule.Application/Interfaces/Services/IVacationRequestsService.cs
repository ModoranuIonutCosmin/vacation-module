using VacationsModule.Application.DTOs;
using VacationsModule.Domain.Datamodels;
using VacationsModule.Domain.Entities;
using VacationsModule.Domain.Models;

namespace VacationsModule.Application.Features;

public interface IVacationRequestsService
{
    Task<CreateVacationRequestResponse> CreateVacationRequest(Guid requestingUserId, CreateVacationRequestRequest request);
    Task<UpdateVacationRequestResponse> UpdateVacationRequest(Guid requestingUserId, UpdateVacationRequestRequest request);
    Task<GetVacationRequestsPaginatedResponse> GetVacationRequestsByStatusAndEmployeeId(Guid requestingUserId,
        GetVacationRequestsPaginatedRequest requestPaginated);

    Task<VacationRequestModel> GetVacationRequestById(Guid requestingUserId, Guid vacationRequestId);
}