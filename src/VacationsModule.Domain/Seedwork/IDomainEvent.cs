using MediatR;

namespace VacationsModule.Domain.Seedwork;

public interface IDomainEvent : INotification
{
    public Guid? EmployeeId { get; set; }
}