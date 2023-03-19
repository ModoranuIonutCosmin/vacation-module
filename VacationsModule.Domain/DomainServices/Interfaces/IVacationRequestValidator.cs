using VacationsModule.Domain.Entities;

namespace VacationsModule.Domain.DomainServices.Interfaces;

public interface IVacationRequestValidator
{
    (bool IsValidated, List<string> Issues) ValidateVacationRequest(VacationRequest vacationRequest);
}