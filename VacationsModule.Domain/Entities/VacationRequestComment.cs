using VacationsModule.Domain.Seedwork;

namespace VacationsModule.Domain.Entities;

public class VacationRequestComment : Entity
{
    public ApplicationUser Author { get; set; }
    public string Message { get; set; }
    public DateTimeOffset PostedAt { get; set; }
    public VacationRequest VacationRequest { get; set; }
}