using VacationsModule.Domain.Seedwork;

namespace VacationsModule.Domain.Entities;

public class VacationRequestInterval : Entity
{
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public VacationRequest VacationRequest { get; set; }
}