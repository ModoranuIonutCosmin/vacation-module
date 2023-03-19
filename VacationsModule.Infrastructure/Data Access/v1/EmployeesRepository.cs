using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VacationsModule.Application.Interfaces.Repositories;
using VacationsModule.Domain.Entities;
using VacationsModule.Infrastructure.Data_Access.Base;

namespace VacationsModule.Infrastructure.Data_Access.v1;

public class EmployeesRepository : Repository<Employee, Guid>, IEmployeesRepository
{
    public EmployeesRepository(VacationRequestsDBContext dbContext,
        IUnitOfWork unitOfWork,
        ILogger<EmployeesRepository> logger) : base(dbContext, logger, unitOfWork)
    {
        
    }


    public async Task<Employee> GetEmployeeByUserIdEagerAsync(Guid userId)
    {
        return await _context.Set<Employee>()
            .Include(e => e.VacationRequests)
            .ThenInclude(vr => vr.VacationIntervals)
            .Include(e => e.VacationRequests)
            .ThenInclude(vr => vr.Comments)
            .Include(e => e.VacationDaysYearlyStatuses)
            .AsSplitQuery()
            .SingleOrDefaultAsync(e => e.Id == userId);

    }
}