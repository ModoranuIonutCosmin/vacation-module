using System.ComponentModel;

namespace VacationsModule.Application.DTOs;

public class GetNationalDaysRequest
{
    public DateTimeOffset? StartDate { get; init; }

    public DateTimeOffset? EndDate { get; init; }
}