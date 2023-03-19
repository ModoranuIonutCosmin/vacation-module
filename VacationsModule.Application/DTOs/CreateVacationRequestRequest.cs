
namespace VacationsModule.Application.DTOs;

public class CreateVacationRequestRequest
{
    // public Guid EmployeeId { get; set; }
    public List<Domain.Models.DateInterval> RequestedDateIntervals { get; set; }
    public string Description { get; set; }
    
}