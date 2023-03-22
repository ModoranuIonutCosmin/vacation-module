using AutoMapper;
using Moq;
using VacationsModule.Application.Auth;
using VacationsModule.Application.DTOs;
using VacationsModule.Application.Features;
using VacationsModule.Application.Interfaces.Repositories;
using VacationsModule.Application.Profiles;
using VacationsModule.Domain.DomainServices.Interfaces;
using VacationsModule.Domain.Entities;
using VacationsModule.Domain.Models;

namespace VacationsModule.UnitTests.Services;

public class VacationRequestsServiceTests
{
    [Test]
    public async Task Given_VacationRequestsService_When_CreateVacationRequestIsCalled_ShouldReturnNotNullVacationRequest()
    {
        var vacationRequestsRepository = new Mock<IVacationRequestsRepository>();
        var vacationRequestsValidator = new Mock<IVacationRequestValidator>();
        var authorizationService = new Mock<IAuthorizationService>();
        var employeeRepository = new Mock<IEmployeesRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new VacationRequestDBToCreateVacationRequestResponseProfile());
                cfg.AddProfile(new DateIntervalModelToVacationDateIntervalDBProfile());
            }));
        
        
        //authorizationService.HasRoles should return true
        authorizationService.Setup(x => x.HasSpecificRoles(It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        
        // ValidateVacationRequest should return true and empty issues list
        vacationRequestsValidator.Setup(x => x.ValidateVacationRequest(It.IsAny<VacationRequest>()))
            .Returns( (IsValid: true, Issues: new List<string>()) );


        var employeeId = Guid.NewGuid();


        var vacationRequestsService = new VacationRequestsService(unitOfWork.Object, employeeRepository.Object,
            vacationRequestsRepository.Object,
            authorizationService.Object, vacationRequestsValidator.Object, mapper);

        var vacationRequest = new CreateVacationRequestRequest()
        {
            Description = "some description",
            RequestedDateIntervals = new List<DateInterval>()
            {
                new DateInterval()
                {
                    StartDate = new DateTime(2021, 1, 1),
                    EndDate = new DateTime(2021, 1, 2)
                }
            },
        };

        var result = await vacationRequestsService.CreateVacationRequest(employeeId, vacationRequest);


        Assert.NotNull(result);
    }
    
    
    [Test]
    public async Task
        Given_VacationRequestsService_When_UpdateVacationRequestIsCalled_ShouldReturnNotNullVacationRequest()
    {
        var vacationRequestsRepository = new Mock<IVacationRequestsRepository>();
        var vacationRequestsValidator = new Mock<IVacationRequestValidator>();
        var authorizationService = new Mock<IAuthorizationService>();
        var employeeRepository = new Mock<IEmployeesRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new VacationRequestDBToCreateVacationRequestResponseProfile());
            cfg.AddProfile(new DateIntervalModelToVacationDateIntervalDBProfile());
        }));
        
        var requestingUser = Guid.NewGuid();
        var employeeThatOwnsRequest = new Employee()
        {
            Id = requestingUser
        };
        
        var existingVacationRequest = new VacationRequest()
        {
            Description = "some description",
            Employee = new Employee()
            {
                Id = requestingUser

            },
            VacationIntervals = new List<VacationRequestInterval>()
            {
                new VacationRequestInterval()
                {
                    StartDate = new DateTime(2021, 1, 1),
                    EndDate = new DateTime(2021, 1, 2)
                }
            }
        };

        vacationRequestsRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingVacationRequest);

        employeeRepository.Setup(x => x.GetEmployeeByUserIdEagerAsync(It.IsAny<Guid>()))
            .ReturnsAsync(employeeThatOwnsRequest);


        //authorizationService.HasRoles should return true
        authorizationService.Setup(x => x.HasSpecificRoles(It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        // ValidateVacationRequest should return true and empty issues list
        vacationRequestsValidator.Setup(x => x.ValidateVacationRequest(It.IsAny<VacationRequest>()))
            .Returns((IsValid: true, Issues: new List<string>()));


        var vacationRequestsService = new VacationRequestsService(unitOfWork.Object, employeeRepository.Object,
            vacationRequestsRepository.Object,
            authorizationService.Object, vacationRequestsValidator.Object, mapper);


        var vacationUpdateRequest = new UpdateVacationRequestRequest()
        {
            Description = "some new description",
            RequestedDateIntervals = new List<DateInterval>()
            {
                new DateInterval()
                {
                    StartDate = new DateTime(2021, 1, 1),
                    EndDate = new DateTime(2021, 1, 2)
                }
            },
        };

        var result = await vacationRequestsService.UpdateVacationRequest(requestingUser, vacationUpdateRequest);

        Assert.NotNull(result);
        Assert.AreEqual("some new description", result.Description);
    }

}