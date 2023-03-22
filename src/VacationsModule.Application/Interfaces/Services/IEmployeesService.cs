using VacationsModule.Application.DTOs;
using VacationsModule.Application.DTOs.Auth;

namespace VacationsModule.Application.Interfaces.Services;

public interface IEmployeesService
{
    Task<RegisterUserDataModelResponse> RegisterEmployeeAsync(
        RegisterEmployeeDataModelRequest registerData);

    Task<GetAvailableVacationDaysResponse> GetAvailableVacationDays(Guid requestingUserId, GetAvailableVacationDaysRequest request);
}