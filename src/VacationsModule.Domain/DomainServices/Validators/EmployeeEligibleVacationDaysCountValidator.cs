using VacationsModule.Domain.DomainServices.Interfaces;
using VacationsModule.Domain.Entities;

namespace VacationsModule.Domain.DomainServices.Validators;

public class EmployeeEligibleVacationDaysCountValidator : IEmployeeEligibleVacationDaysCountValidator
{

    
    public (bool IsValidated, List<string> Issues) Validate(Employee employee, int requestedVacationDaysCount)
    {
        var issues = new List<string>();
        var isValidated = true;
        
        var currentDate = DateTimeOffset.UtcNow;

        var currentYearVacationDaysStatus = employee.VacationDaysYearlyStatuses
            .FirstOrDefault(v => v.YearStartDate <= currentDate && currentDate <= v.YearEndDate);
        
        int availableVacationDays = currentYearVacationDaysStatus?.LeftVacationDays ?? 0;
        
        if (availableVacationDays < requestedVacationDaysCount)
        {
            isValidated = false;
            issues.Add("Employee does not have enough vacation days");
        }
        
        return (isValidated, issues);
    }
}