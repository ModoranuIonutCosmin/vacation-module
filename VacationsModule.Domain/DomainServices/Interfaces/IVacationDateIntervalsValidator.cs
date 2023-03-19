using VacationsModule.Domain.Entities;

namespace VacationsModule.Domain.DomainServices.Interfaces;

public interface IVacationDateIntervalsValidator
{
    (bool IsValidated, List<string> Issues) Validate(List<VacationRequestInterval> dateIntervals);
}