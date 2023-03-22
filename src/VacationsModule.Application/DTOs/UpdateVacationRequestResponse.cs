namespace VacationsModule.Application.DTOs;

public class UpdateVacationRequestResponse
{
    public Guid VacationRequestId { get; set; }
    public Guid EmployeeId { get; set; }
    public List<Domain.Models.DateInterval> RequestedDateIntervals { get; set; }
    
    public DateTimeOffset LastUpdate { get; set; }
    public string Description { get; set; } 
}