using VacationsModule.Domain.DomainServices.Interfaces;
using VacationsModule.Domain.Entities;
using VacationsModule.Domain.Models;

namespace VacationsModule.Domain.DomainServices;

public class VacationDaysCalculatorService : IVacationDaysCalculatorService
{
    private readonly INationalDaysService _nationalDaysService;

    public VacationDaysCalculatorService(INationalDaysService nationalDaysService)
    {
        _nationalDaysService = nationalDaysService;
    }
    
    public int CalculateVacationDaysForRequestedIntervals(List<VacationRequestInterval> requestedVacationIntervals)
    {
        DateTimeOffset startDate = requestedVacationIntervals.Min(e => e.StartDate);
        DateTimeOffset endDate = requestedVacationIntervals.Max(e => e.EndDate);
        
        var nationalDays = _nationalDaysService.GetNationalDays(startDate, endDate);
        var vacationDays = 0;
        
        foreach (var interval in requestedVacationIntervals)
        {
            vacationDays += CalculateVacationDays(nationalDays, interval.StartDate, interval.EndDate);
        }

        return vacationDays;
    }

    private int CalculateVacationDays(List<NationalDay> nationalDays, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var days = (endDate - startDate).Days;
        var nationalDaysCount = nationalDays.Count;
        var vacationDays = days - nationalDaysCount;
        
        return vacationDays;
    }
}