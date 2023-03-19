using VacationsModule.Application.DTOs;
using VacationsModule.Application.DTOs.Auth;

namespace VacationsModule.Application.Features;

public interface IEmployeesService
{
    Task<RegisterUserDataModelResponse> RegisterEmployeeAsync(
        RegisterEmployeeDataModelRequest registerData);

    Task<GetAvailableVacationDaysResponse> GetAvailableVacationDays(Guid requestingUserId, GetAvailableVacationDaysRequest request);
}