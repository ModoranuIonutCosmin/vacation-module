using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Moq;
using VacationsModule.Application.DTOs;
using VacationsModule.Application.Features;
using VacationsModule.Application.Interfaces.Repositories;
using VacationsModule.Domain.Entities;
using IAuthorizationService = VacationsModule.Application.Auth.IAuthorizationService;

namespace VacationsModule.UnitTests.Services;

public class EmployeesServiceTests
{
    
    [SetUp]
    public void Setup()
    {
                
    }
    
    [Test]
    public async Task Given_EmployeesService_When_GetAvailableVacationDaysIsCalled_ShouldReturnAvailableVacationDays()
    {
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            VacationDaysYearlyStatuses = new List<VacationDaysStatus>
            {
                new VacationDaysStatus
                {
                    Id = Guid.NewGuid(),
                    YearStartDate = DateTimeOffset.UtcNow,
                    YearEndDate = DateTimeOffset.UtcNow.AddYears(1),
                    Year = DateTimeOffset.UtcNow.Year,
                    LeftVacationDays = 14
                }
            }
        };
        
        var employeeRepositoryMock = new Mock<IEmployeesRepository>();
        employeeRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(employee);
        
        // mock authorization service
        
        var authorizationServiceMock = new Mock<IAuthorizationService>();
        
        authorizationServiceMock.Setup(x => x.HasSpecificRoles(It.IsAny<Guid>(), It.IsAny<string[]>()))
            .ReturnsAsync(true);
        
        var employeesService = new EmployeesService(default, 
            authorizationServiceMock.Object, 
            employeeRepositoryMock.Object);
        
        var result = await employeesService.GetAvailableVacationDays(
            Guid.NewGuid(),
            new GetAvailableVacationDaysRequest
            {
                Year = DateTimeOffset.UtcNow.Year
            });
        
        Assert.AreEqual(14, result.AvailableVacationDays);
    }
    
    
}