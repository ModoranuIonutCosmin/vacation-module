using VacationsModule.Domain.Entities;

namespace VacationsModule.Domain.DomainServices.Interfaces;

public interface IVacationDaysCalculatorService
{
    int CalculateVacationDaysForRequestedIntervals(List<VacationRequestInterval> requestedVacationIntervals);
}