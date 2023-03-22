using VacationsModule.Domain.DomainServices.Validators;
using VacationsModule.Domain.Entities;

namespace VacationsModule.UnitTests.Domain;

public class VacationDateIntervalsValidatorTest
{
    
    [SetUp]
    public void Setup()
    {
        
    }
    
    [Test]
    public void Given_VacationDateIntervalsValidator_When_SingleValidVacationIntervalInTheFutureProvided_ShouldValidateAndReturnTrue()
    {
        var vacationDateIntervalsValidator = new VacationDateIntervalsValidator();
        
        var (isValidated, issues) = vacationDateIntervalsValidator.Validate(
            new List<VacationRequestInterval>
            {
                new VacationRequestInterval
                {
                    Id = Guid.NewGuid(),
                    StartDate = DateTimeOffset.UtcNow.AddDays(30),
                    EndDate = DateTimeOffset.UtcNow.AddDays(31)
                }
            });
        
        Assert.IsTrue(isValidated);
        Assert.AreEqual(0, issues.Count);
    }
    
    [Test]
    public void Given_VacationDateIntervalsValidator_When_OverlappingVacationIntervalInTheFuture_ShouldNotValidate()
    {
        var vacationDateIntervalsValidator = new VacationDateIntervalsValidator();
        
        var (isValidated, issues) = vacationDateIntervalsValidator.Validate(
            new List<VacationRequestInterval>
            {
                new VacationRequestInterval
                {
                    Id = Guid.NewGuid(),
                    StartDate = DateTimeOffset.UtcNow.AddDays(20),
                    EndDate = DateTimeOffset.UtcNow.AddDays(31)
                },
                new VacationRequestInterval
                {
                    Id = Guid.NewGuid(),
                    StartDate = DateTimeOffset.UtcNow.AddDays(25),
                    EndDate = DateTimeOffset.UtcNow.AddDays(32)
                },
                
            });
        
        Assert.IsFalse(isValidated);
        Assert.AreEqual(1, issues.Count);
    }
    
    // given interval in the past should not validate

    [Test]
    public void Given_VacationDateIntervalsValidator_When_IntervalInThePastProvided_ShouldNotValidate()
    {
        var vacationDateIntervalsValidator = new VacationDateIntervalsValidator();
        
        var (isValidated, issues) = vacationDateIntervalsValidator.Validate(
            new List<VacationRequestInterval>
            {
                new VacationRequestInterval
                {
                    Id = Guid.NewGuid(),
                    StartDate = DateTimeOffset.UtcNow.AddDays(-30),
                    EndDate = DateTimeOffset.UtcNow.AddDays(-29)
                }
            });
        
        Assert.IsFalse(isValidated);
        Assert.AreEqual(1, issues.Count);
    }
    
    //Given interval with start date after end date should not validate
    
    [Test]
    public void Given_VacationDateIntervalsValidator_When_IntervalWithStartDateAfterEndDateProvided_ShouldNotValidate()
    {
        var vacationDateIntervalsValidator = new VacationDateIntervalsValidator();
        
        var (isValidated, issues) = vacationDateIntervalsValidator.Validate(
            new List<VacationRequestInterval>
            {
                new VacationRequestInterval
                {
                    Id = Guid.NewGuid(),
                    StartDate = DateTimeOffset.UtcNow.AddDays(30),
                    EndDate = DateTimeOffset.UtcNow.AddDays(29)
                }
            });
        
        Assert.IsFalse(isValidated);
        Assert.AreEqual(1, issues.Count);
    }
}
