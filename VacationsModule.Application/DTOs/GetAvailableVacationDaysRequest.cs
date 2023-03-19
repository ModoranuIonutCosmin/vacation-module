namespace VacationsModule.Application.DTOs;

public class GetAvailableVacationDaysRequest
{
    public int Year { get; init; } = DateTimeOffset.UtcNow.Year;
}