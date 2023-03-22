using VacationsModule.Domain.Seedwork;

namespace VacationsModule.Domain.Entities;

public class VacationDaysStatus : Entity
{
    public int LeftVacationDays { get; set; }
    public int TotalVacationDays { get; set; }
    public int Year { get; set; }
    public DateTimeOffset YearStartDate { get; set; } // la fel ca Employee.EmploymentDate
    public DateTimeOffset YearEndDate { get; set; } // starsitul perioadei in care se considera alea n zile de vacanta
    public Employee Employee { get; set; }
}