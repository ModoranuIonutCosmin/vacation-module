using VacationsModule.Domain.DomainServices.Interfaces;
using VacationsModule.Domain.Entities;

namespace VacationsModule.Domain.DomainServices.Validators;

public class VacationRequestValidator : IVacationRequestValidator
{
    private readonly IVacationDateIntervalsValidator _dateIntervalsValidator;
    private readonly IEmployeeEligibleVacationDaysCountValidator _employeeEligibleVacationDaysCountValidator;
    private readonly IVacationDaysCalculatorService _vacationDaysCalculatorService;
    
    public VacationRequestValidator(IVacationDateIntervalsValidator dateIntervalsValidator,
        IEmployeeEligibleVacationDaysCountValidator employeeEligibleVacationDaysCountValidator,
        IVacationDaysCalculatorService vacationDaysCalculatorService)
    {
        _dateIntervalsValidator = dateIntervalsValidator;
        _employeeEligibleVacationDaysCountValidator = employeeEligibleVacationDaysCountValidator;
        _vacationDaysCalculatorService = vacationDaysCalculatorService;
    }

    public (bool IsValidated, List<string> Issues) ValidateVacationRequest(VacationRequest vacationRequest)
    {
        var issues = new List<string>();
        
        if (vacationRequest == null)
        {
            return (false, new List<string> {"Vacation request is null"});
        }
        
        if (vacationRequest.Employee == null)
        {
            return (false, new List<string> {"Employee is null"});
        }
        
        (bool IsValidated, List<string> Issues) dateIntervalsValidationResult 
            = _dateIntervalsValidator.Validate(vacationRequest.VacationIntervals);

        if (!dateIntervalsValidationResult.IsValidated)
        {
            issues.AddRange(dateIntervalsValidationResult.Issues);
        }
        
        int vacationDaysRequested = _vacationDaysCalculatorService
            .CalculateVacationDaysForRequestedIntervals(vacationRequest.VacationIntervals);
        
        var employeeVacationEligibilityValidationResult = _employeeEligibleVacationDaysCountValidator
            .Validate(vacationRequest.Employee, vacationDaysRequested);

        if (!employeeVacationEligibilityValidationResult.IsValidated)
        {
            issues.AddRange(employeeVacationEligibilityValidationResult.Issues);
        }
        
        return (IsValidated: !issues.Any(), issues);
    }
}