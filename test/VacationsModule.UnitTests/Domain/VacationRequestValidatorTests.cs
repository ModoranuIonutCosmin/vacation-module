using Moq;
using VacationsModule.Domain.DomainServices.Interfaces;
using VacationsModule.Domain.DomainServices.Validators;
using VacationsModule.Domain.Entities;

namespace VacationsModule.UnitTests.Domain;

public class VacationRequestValidatorTests
{
    [Test]
    public void Given_VacationRequestValidator_When_VacationRequestWithValidIntervalsAndSufficientVacationDays_ShouldValidateAndAddIssue()
    {
        // mock vacation days calculator service to return 5 days
        var vacationDaysCalculatorServiceMock = new Mock<IVacationDaysCalculatorService>();
        vacationDaysCalculatorServiceMock.Setup(x => x.CalculateVacationDaysForRequestedIntervals(It.IsAny<List<VacationRequestInterval>>()))
            .Returns(5);
        
        // mock employee eligible vacation days count validator to return true
        var employeeEligibleVacationDaysCountValidatorMock = new Mock<IEmployeeEligibleVacationDaysCountValidator>();
        employeeEligibleVacationDaysCountValidatorMock.Setup(x => x.Validate(It.IsAny<Employee>(), It.IsAny<int>()))
            .Returns((true, new List<string>()));
        
        // mock vacation date intervals validator to return true
        var vacationDateIntervalsValidatorMock = new Mock<IVacationDateIntervalsValidator>();
        vacationDateIntervalsValidatorMock.Setup(x => x.Validate(It.IsAny<List<VacationRequestInterval>>()))
            .Returns((true, new List<string>()));
        
        var vacationRequest = new VacationRequest
        {
            Id = Guid.NewGuid(),
            Employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                VacationDaysYearlyStatuses = new List<VacationDaysStatus>
                {
                    new VacationDaysStatus()
                    {
                        Id = Guid.NewGuid(),
                        YearStartDate = DateTimeOffset.UtcNow,
                        YearEndDate = DateTimeOffset.UtcNow.AddYears(1),
                        LeftVacationDays = 10
                    }
                }
            },
            VacationIntervals = new List<VacationRequestInterval>
            {
                new VacationRequestInterval
                {
                    Id = Guid.NewGuid(),
                    StartDate = DateTimeOffset.UtcNow,
                    EndDate = DateTimeOffset.UtcNow.AddDays(5)
                }
            }
        };
        
        var vacationRequestValidator = new VacationRequestValidator(vacationDateIntervalsValidatorMock.Object,
            employeeEligibleVacationDaysCountValidatorMock.Object, vacationDaysCalculatorServiceMock.Object);
        
        var (isValidated, issues) = vacationRequestValidator.ValidateVacationRequest(vacationRequest);
        
        Assert.IsTrue(isValidated);
        Assert.IsEmpty(issues);
    }
    
    // same test but user should not have enough vacation days
    [Test]
    public void Given_VacationRequestValidator_When_VacationRequestWithValidIntervalsAndInsufficientVacationDays_ShouldNotValidateAndAddIssue()
    {
        // mock vacation days calculator service to return 15 days
        var vacationDaysCalculatorServiceMock = new Mock<IVacationDaysCalculatorService>();
        vacationDaysCalculatorServiceMock.Setup(x => x.CalculateVacationDaysForRequestedIntervals(It.IsAny<List<VacationRequestInterval>>()))
            .Returns(15);
        
        // mock employee eligible vacation days count validator to return true
        var employeeEligibleVacationDaysCountValidatorMock = new Mock<IEmployeeEligibleVacationDaysCountValidator>();
        employeeEligibleVacationDaysCountValidatorMock.Setup(x => x.Validate(It.IsAny<Employee>(), It.IsAny<int>()))
            .Returns((false, new List<string> {"Insufficient vacation days"}));
        
        // mock vacation date intervals validator to return true
        var vacationDateIntervalsValidatorMock = new Mock<IVacationDateIntervalsValidator>();
        vacationDateIntervalsValidatorMock.Setup(x => x.Validate(It.IsAny<List<VacationRequestInterval>>()))
            .Returns((true, new List<string>()));
        
        var vacationRequest = new VacationRequest
        {
            Id = Guid.NewGuid(),
            Employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                VacationDaysYearlyStatuses = new List<VacationDaysStatus>
                {
                    new VacationDaysStatus()
                    {
                        Id = Guid.NewGuid(),
                        YearStartDate = DateTimeOffset.UtcNow,
                        YearEndDate = DateTimeOffset.UtcNow.AddYears(1),
                        LeftVacationDays = 10
                    }
                }
            },
            VacationIntervals = new List<VacationRequestInterval>
            {
                new VacationRequestInterval
                {
                    Id = Guid.NewGuid(),
                    StartDate = DateTimeOffset.UtcNow,
                    EndDate = DateTimeOffset.UtcNow.AddDays(5)
                }
            }
        };
        
        var vacationRequestValidator = new VacationRequestValidator(vacationDateIntervalsValidatorMock.Object,
            employeeEligibleVacationDaysCountValidatorMock.Object, vacationDaysCalculatorServiceMock.Object);
        
        var (isValidated, issues) = vacationRequestValidator.ValidateVacationRequest(vacationRequest);
        
        Assert.IsFalse(isValidated);
        Assert.AreEqual(1, issues.Count);
    }
    
    // same test but user should have invalid vacation intervals
    [Test]
    public void Given_VacationRequestValidator_When_VacationRequestWithInvalidIntervalsAndSufficientVacationDays_ShouldNotValidateAndAddIssue()
    {
        // mock vacation days calculator service to return 5 days
        var vacationDaysCalculatorServiceMock = new Mock<IVacationDaysCalculatorService>();
        vacationDaysCalculatorServiceMock.Setup(x => x.CalculateVacationDaysForRequestedIntervals(It.IsAny<List<VacationRequestInterval>>()))
            .Returns(5);
        
        // mock employee eligible vacation days count validator to return true
        var employeeEligibleVacationDaysCountValidatorMock = new Mock<IEmployeeEligibleVacationDaysCountValidator>();
        employeeEligibleVacationDaysCountValidatorMock.Setup(x => x.Validate(It.IsAny<Employee>(), It.IsAny<int>()))
            .Returns((true, new List<string>()));
        
        // mock vacation date intervals validator to return true
        var vacationDateIntervalsValidatorMock = new Mock<IVacationDateIntervalsValidator>();
        vacationDateIntervalsValidatorMock.Setup(x => x.Validate(It.IsAny<List<VacationRequestInterval>>()))
            .Returns((false, new List<string> {"Invalid vacation intervals"}));
        
        var vacationRequest = new VacationRequest
        {
            Id = Guid.NewGuid(),
            Employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                VacationDaysYearlyStatuses = new List<VacationDaysStatus>
                {
                    new VacationDaysStatus()
                    {
                        Id = Guid.NewGuid(),
                        YearStartDate = DateTimeOffset.UtcNow,
                        YearEndDate = DateTimeOffset.UtcNow.AddYears(1),
                        LeftVacationDays = 10
                    }
                }
            },
            VacationIntervals = new List<VacationRequestInterval>
            {
                new VacationRequestInterval
                {
                    Id = Guid.NewGuid(),
                    StartDate = DateTimeOffset.UtcNow,
                    EndDate = DateTimeOffset.UtcNow.AddDays(5)
                }
            }
        };
        
        var vacationRequestValidator = new VacationRequestValidator(vacationDateIntervalsValidatorMock.Object,
            employeeEligibleVacationDaysCountValidatorMock.Object, vacationDaysCalculatorServiceMock.Object);
        
        var (isValidated, issues) = vacationRequestValidator.ValidateVacationRequest(vacationRequest);
        
        Assert.IsFalse(isValidated);
        Assert.AreEqual(1, issues.Count);
    }
    
    
}