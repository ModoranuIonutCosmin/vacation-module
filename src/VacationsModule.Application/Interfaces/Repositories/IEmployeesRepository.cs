using VacationsModule.Application.Interfaces.Repositories.Base;
using VacationsModule.Domain.Entities;

namespace VacationsModule.Application.Interfaces.Repositories;

public interface IEmployeesRepository : IRepository<Employee, Guid>
{
    
    public Task<Employee> GetEmployeeByUserIdEagerAsync(Guid userId);

    // public Task<Employee> AddEmployeeVacationDaysStatus(Guid userId);
}