using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VacationsModule.Application.DTOs.Auth;
using VacationsModule.Application.Features;
using VacationsModule.Application.Interfaces.Services;
using VacationsModule.WebApi.Controllers;

namespace VacationsModule.UnitTests.API;

public class EmployeeControllerTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task
        Given_EmployeeController_When_RegisterEmployeeAsyncIsCalledAndEmployeeServiceRegisterEmployeeAsyncThrowsInvalidOperationException_ShouldReturnForbidden()
    {
        var registerData = new RegisterEmployeeDataModelRequest()
        {
            Department = "Department",
            Email = "Email",
            EmploymentDate = DateTimeOffset.Now,
            FirstName = "FirstName",
            LastName = "LastName",
            Password = "Password",
            Position = "Position",
            UserName = "UserName"
        };

        var employeeService = Mock.Of<IEmployeesService>();

        Mock.Get(employeeService)
            .Setup(x => x.RegisterEmployeeAsync(It.IsAny<RegisterEmployeeDataModelRequest>()))
            .ThrowsAsync(new InvalidOperationException(""));

        var employeeController = new EmployeeController(logger: null, employeeService);

        var result = await employeeController.RegisterEmployeeAsync(registerData: registerData);

        result.Should().BeOfType<ObjectResult>()
            .And.Subject.As<ObjectResult>().StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }
}