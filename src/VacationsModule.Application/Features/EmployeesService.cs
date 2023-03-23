using System.Security.Authentication;
using Microsoft.AspNetCore.Identity;
using VacationsModule.Application.Auth;
using VacationsModule.Application.Auth.ExtensionMethods;
using VacationsModule.Application.DTOs;
using VacationsModule.Application.DTOs.Auth;
using VacationsModule.Application.Interfaces.Repositories;
using VacationsModule.Application.Interfaces.Services;
using VacationsModule.Domain.Datamodels;
using VacationsModule.Domain.Entities;
using VacationsModule.Domain.Exceptions;

namespace VacationsModule.Application.Features;

public class EmployeesService : IEmployeesService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthorizationService _authorizationService;
    private readonly IEmployeesRepository _employeesRepository;

    private int MAX_VACATION_DAYS = 25;
    private TimeSpan PERIOD_OF_MAX_VACATION_DAYS = TimeSpan.FromDays(365);


    public EmployeesService(UserManager<ApplicationUser> userManager, IAuthorizationService authorizationService,
        IEmployeesRepository employeesRepository)
    {
        _userManager = userManager;
        _authorizationService = authorizationService;
        _employeesRepository = employeesRepository;
    }

    public async Task<RegisterUserDataModelResponse> RegisterEmployeeAsync(
        RegisterEmployeeDataModelRequest registerData)
    {
        if (registerData == null || string.IsNullOrWhiteSpace(registerData.UserName))
            throw new InvalidOperationException(
                "Bad format for request! Please provide every detail required for registration!");

        var user = new Employee()
        {
            UserName = registerData.UserName,
            FirstName = registerData.FirstName,
            LastName = registerData.LastName,
            Email = registerData.Email,
            EmploymentDate = registerData.EmploymentDate,
            VacationDaysYearlyStatuses = new List<VacationDaysStatus>()
            {
                new VacationDaysStatus()
                {
                    Year = registerData.EmploymentDate.Year,
                    LeftVacationDays = MAX_VACATION_DAYS,
                    YearStartDate = registerData.EmploymentDate,
                    YearEndDate = registerData.EmploymentDate.Add(PERIOD_OF_MAX_VACATION_DAYS).AddDays(-1),
                    TotalVacationDays = MAX_VACATION_DAYS
                }
            },
            Department = registerData.Department,
            Position = registerData.Position
        };

        var result = await _userManager.CreateAsync(user, registerData.Password);

        if (!result.Succeeded)
            throw new AuthenticationException(result.Errors.AggregateErrors());

        await _userManager.AddToRoleAsync(user, RolesEnum.Employee.ToString());

        var userIdentity = await _userManager.FindByNameAsync(user.UserName);

        return new RegisterUserDataModelResponse
        {
            UserName = userIdentity.UserName,
            FirstName = userIdentity.FirstName,
            LastName = userIdentity.LastName,
            Email = userIdentity.Email,
            Token = "Bearer"
        };
    }

    public async Task<GetAvailableVacationDaysResponse> GetAvailableVacationDays(Guid requestingUserId, GetAvailableVacationDaysRequest request)
    {

        var employeeId = requestingUserId;

        if (!await _authorizationService.HasSpecificRoles(employeeId, RolesEnum.Employee.ToString()))
        {
            throw new UnauthorizedAccessException("You are not authorized to see details about vacation days!");
        }

        Employee employee = await _employeesRepository.GetEmployeeByUserIdEagerAsync(employeeId);
        int queryYear = request.Year;
        int currentYear = DateTimeOffset.UtcNow.Year;

        var currentYearVacationDaysStatus = employee.VacationDaysYearlyStatuses
            .FirstOrDefault(v => v.Year == currentYear);

        var queryYearVacationDaysStatus = employee.VacationDaysYearlyStatuses
            .FirstOrDefault(v => v.Year == queryYear);

        var lastVacationYearExpired = currentYearVacationDaysStatus == null || currentYearVacationDaysStatus.YearEndDate < DateTimeOffset.UtcNow;

        if (lastVacationYearExpired)
        {
            var employmentDate = employee.EmploymentDate;
            var yearsDiff = currentYear - employmentDate.Year;
            var thisYearVacationDaysReplenish = employmentDate.AddYears(yearsDiff);

            currentYearVacationDaysStatus = new VacationDaysStatus()
            {
                Year = currentYear,
                LeftVacationDays = MAX_VACATION_DAYS,
                YearStartDate = thisYearVacationDaysReplenish,
                YearEndDate = thisYearVacationDaysReplenish.AddYears(1).AddDays(-1),
                TotalVacationDays = MAX_VACATION_DAYS
            };

            employee.VacationDaysYearlyStatuses.Add(currentYearVacationDaysStatus);

            await _employeesRepository.UpdateAsync(employee);
        }

        if (currentYearVacationDaysStatus.Year == queryYear)
        {
            return new GetAvailableVacationDaysResponse
            {
                AvailableVacationDays = currentYearVacationDaysStatus.LeftVacationDays
            };
        }

        if (queryYearVacationDaysStatus != null)
        {
            return new GetAvailableVacationDaysResponse
            {
                AvailableVacationDays = queryYearVacationDaysStatus?.LeftVacationDays ?? 0
            };
        }
        
        throw new NoVacationDaysLogsException($"No vacation days logs for year {queryYear} found!");
    }
}