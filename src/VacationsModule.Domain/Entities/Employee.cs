namespace VacationsModule.Domain.Entities;

//table per hierarchy
public class Employee : ApplicationUser
{
    public DateTimeOffset EmploymentDate { get; set; }
    public string? Position { get; set; }
    public string? Department { get; set; }
    public List<VacationDaysStatus> VacationDaysYearlyStatuses { get; set; }
    public List<VacationRequest> VacationRequests { get; set; }
}