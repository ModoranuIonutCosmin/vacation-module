using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VacationsModule.Application.Interfaces.Repositories;
using VacationsModule.Domain.Datamodels;
using VacationsModule.Domain.Entities;
using VacationsModule.Infrastructure.Data_Access.Base;

namespace VacationsModule.Infrastructure.Data_Access.v1;

public class VacationRequestsRepository : Repository<VacationRequest, Guid>, IVacationRequestsRepository
{
    private readonly ILogger<VacationRequestsRepository> _logger;

    public VacationRequestsRepository(VacationRequestsDBContext vacationRequestsDbContext, 
        ILogger<VacationRequestsRepository> logger, 
        IUnitOfWork unitOfWork)
        : base(vacationRequestsDbContext, logger, unitOfWork)
    {
        _logger = logger;
    }
    

    public async Task<List<VacationRequest>> GetVacationRequestsByStatusAndEmployeeIdPaginated(VacationRequestStatus? status, 
        Guid? employeeId,
        int page = 0,
        int pageSize = 10)
    {

        IQueryable<VacationRequest> query = _context.VacationRequests
            .Include(vr => vr.Employee)
            .Include(vr => vr.VacationIntervals)
            .Include(vr => vr.Comments);
        
        if (status.HasValue && status != VacationRequestStatus.ALL)
        {
            query = query.Where(vr => vr.Status == status);
        }

        if (employeeId.HasValue)
        {
            query = query.Where(vr => vr.Employee.Id.Equals(employeeId));
        }
        
        return await query
            .OrderByDescending(vr => vr.CreatedAt)
            .ThenByDescending(vr => vr.Id)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<VacationRequest> GetVacationRequestByEmployeeIdAndVacationRequestId(Guid vacationRequestId, Guid employeeId)
    {
        return await _context.VacationRequests
            .Include(vr => vr.Employee)
            .Include(vr => vr.VacationIntervals)
            .Include(vr => vr.Comments)
            .SingleOrDefaultAsync(vr => vr.Employee.Id.Equals(employeeId) && vr.Id.Equals(vacationRequestId));
    }
}