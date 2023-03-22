using VacationsModule.Application.Interfaces.Repositories.Base;
using VacationsModule.Domain.Datamodels;
using VacationsModule.Domain.Entities;

namespace VacationsModule.Application.Interfaces.Repositories;

public interface IVacationRequestsRepository : IRepository<VacationRequest, Guid>
{
    Task<List<VacationRequest>> GetVacationRequestsByStatusAndEmployeeIdPaginated(VacationRequestStatus? status, Guid? employeeId,
        int page = 0, int pageSize = 10);

    Task<VacationRequest> GetVacationRequestByEmployeeIdAndVacationRequestId(Guid vacationRequestId,
        Guid employeeId);
}