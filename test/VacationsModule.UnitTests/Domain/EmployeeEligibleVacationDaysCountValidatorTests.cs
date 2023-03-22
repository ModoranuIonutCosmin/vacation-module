using VacationsModule.Domain.DomainServices.Validators;
using VacationsModule.Domain.Entities;

namespace VacationsModule.UnitTests.Domain;

public class EmployeeEligibleVacationDaysCountValidatorTests
{
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void Given_EmployeeEligibleVacationDaysCountValidator_When_EmployeeWithInsufficientVacationDaysIsPassed_ShouldNotValidateAndAddIssue()
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
                    LeftVacationDays = 10
                }
            }
        };
        
        var employeeEligibleVacationDaysCountValidator = new EmployeeEligibleVacationDaysCountValidator();
        
        var (isValidated, issues) = employeeEligibleVacationDaysCountValidator.Validate(employee, 11);
        
        Assert.IsFalse(isValidated);
        Assert.AreEqual(1, issues.Count);
    }
    
    [Test]
    public void Given_EmployeeEligibleVacationDaysCountValidator_When_EmployeeWithSufficientVacationDaysIsPassed_ShouldValidateAndReturnTrue()
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
                    LeftVacationDays = 14
                }
            }
        };
        
        var employeeEligibleVacationDaysCountValidator = new EmployeeEligibleVacationDaysCountValidator();
        
        var (isValidated, issues) = employeeEligibleVacationDaysCountValidator.Validate(employee,
            requestedVacationDaysCount: 11);
        
        Assert.IsTrue(isValidated);
    }
}