using VacationsModule.Application.DTOs;
using VacationsModule.Domain.Models;

namespace VacationsModule.Application.Interfaces.Services;

public interface IVacationRequestsService
{
    Task<CreateVacationRequestResponse> CreateVacationRequest(Guid requestingUserId, CreateVacationRequestRequest request);
    Task<UpdateVacationRequestResponse> UpdateVacationRequest(Guid requestingUserId, UpdateVacationRequestRequest request);
    Task<GetVacationRequestsPaginatedResponse> GetVacationRequestsByStatusAndEmployeeId(Guid requestingUserId,
        GetVacationRequestsPaginatedRequest requestPaginated);

    Task<VacationRequestModel> GetVacationRequestById(Guid requestingUserId, Guid vacationRequestId);
}