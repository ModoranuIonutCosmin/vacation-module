using VacationsModule.Domain.Datamodels;
using VacationsModule.Domain.Seedwork;

namespace VacationsModule.Domain.Entities;

public class VacationRequest : Entity, IAggregateRoot
{
    public Employee Employee { get; set; }
    public Manager? Approver { get; set; }
    public VacationRequestStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public List<VacationRequestInterval> VacationIntervals { get; set; }
    public List<VacationRequestComment> Comments { get; set; }
    
    // poate si snapshot-uri la toatele detaliile request-ului atunci cand facem update
}