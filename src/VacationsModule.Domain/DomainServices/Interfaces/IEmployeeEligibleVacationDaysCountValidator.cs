using VacationsModule.Domain.Entities;

namespace VacationsModule.Domain.DomainServices.Interfaces;

public interface IEmployeeEligibleVacationDaysCountValidator
{
    (bool IsValidated, List<string> Issues) Validate(Employee employee, int requestedVacationDaysCount);
}