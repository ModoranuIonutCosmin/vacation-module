using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VacationsModule.Domain.Seedwork;

public class Entity
{
    public Guid Id { get; set; }

    [NotMapped] [JsonIgnore] public List<IDomainEvent> DomainEvents { get; } = new();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        DomainEvents.Add(domainEvent);
    }
}