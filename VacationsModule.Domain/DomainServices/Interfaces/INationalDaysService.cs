using VacationsModule.Domain.Models;

namespace VacationsModule.Domain.DomainServices.Interfaces;

public interface INationalDaysService
{
    List<NationalDay> GetNationalDays(DateTimeOffset? startDate, DateTimeOffset? endDate);
}