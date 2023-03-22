using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VacationsModule.Application.DTOs;
using VacationsModule.Application.Features;
using VacationsModule.Application.Interfaces.Services;
using VacationsModule.Domain.Entities;
using VacationsModule.Domain.Exceptions;
using VacationsModule.UnitTests.Auth_mock;
using VacationsModule.WebApi.Controllers;

namespace VacationsModule.UnitTests.API;

public class VacationRequestsControllerTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task
        Given_VacationRequestsController_When_GetVacationRequestsAsyncIsCalledAndVacationRequestsServiceGetVacationRequestsAsync_ShouldReturnOk200()
    {
        var vacationRequestsServiceMock = Mock.Of<IVacationRequestsService>();

        Mock.Get(vacationRequestsServiceMock)
            .Setup(x => x.GetVacationRequestsByStatusAndEmployeeId(It.IsAny<Guid>(),
                It.IsAny<GetVacationRequestsPaginatedRequest>()))
            .ReturnsAsync(new GetVacationRequestsPaginatedResponse());

        VacationRequestsController vacationRequestsController = MockHttpContext.GetControllerWithAuthContext<VacationRequestsController>(new ApplicationUser()
        {
            Id = Guid.NewGuid(),
            UserName = "UserName",
            Email = "Email",
            FirstName = "FirstName",
            LastName = "LastName",
        });

        var result = await vacationRequestsController.GetVacationRequests(new GetVacationRequestsPaginatedRequest(),
            vacationRequestsServiceMock);

        result.Should().BeOfType<OkObjectResult>();
    }
}