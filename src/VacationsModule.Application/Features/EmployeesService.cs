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
        
        await _userManager.AddToRoleAsync(user,  RolesEnum.Employee.ToString());

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
        
        if (! await _authorizationService.HasSpecificRoles(employeeId, RolesEnum.Employee.ToString()))
        {
            throw new UnauthorizedAccessException("You are not authorized to create vacation requests!");
        }
        
        Employee employee = await _employeesRepository.GetByIdAsync(employeeId);
        int queryYear = request.Year;
        
        var currentYearVacationDaysStatus = employee.VacationDaysYearlyStatuses
            .FirstOrDefault(v => v.Year == queryYear);
        
        int availableVacationDays = currentYearVacationDaysStatus?.LeftVacationDays ?? 0;
        
        return new GetAvailableVacationDaysResponse
        {
            AvailableVacationDays = availableVacationDays
        };
    }
}