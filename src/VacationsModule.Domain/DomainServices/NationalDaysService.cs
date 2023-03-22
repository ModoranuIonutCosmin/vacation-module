using PublicHoliday;
using VacationsModule.Domain.DomainServices.Interfaces;
using VacationsModule.Domain.Models;

namespace VacationsModule.Domain.DomainServices;

public class NationalDaysService : INationalDaysService
{
    public List<NationalDay> GetNationalDays(DateTimeOffset? startDate, DateTimeOffset? endDate)
    {
        var currentDate = DateTimeOffset.UtcNow;
        
        if (startDate == null)
        {
            startDate = new DateTimeOffset(currentDate.Year, 1, 1, 0, 0, 0, currentDate.Offset);
        }
        
        if (endDate == null)
        {
            endDate = new DateTimeOffset(currentDate.Year, 12, 31, 0, 0, 0, currentDate.Offset);
        }

        var nationalDays = new RomanianPublicHoliday().PublicHolidayNames(2023)
            .Where(e => e.Key >= startDate && e.Key <= endDate);

        return nationalDays.Select(e => new NationalDay
        {
            Date = new DateTimeOffset(e.Key),
            Name = e.Value
        }).ToList();
    }
}