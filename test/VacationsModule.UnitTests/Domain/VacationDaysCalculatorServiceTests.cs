using Moq;
using VacationsModule.Domain.DomainServices;
using VacationsModule.Domain.DomainServices.Interfaces;
using VacationsModule.Domain.Entities;
using VacationsModule.Domain.Models;

namespace VacationsModule.UnitTests.Domain;

public class VacationDaysCalculatorServiceTests
{
    [Test]
    public void Given_VacationDaysCalculatorService_When_CalculateVacationDaysForRequestedIntervalsIsCalled_ShouldReturnCorrectVacationDaysCount()
    {
        var nationalDaysService = new Mock<INationalDaysService>();
        
        nationalDaysService.Setup(x => x.GetNationalDays(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
            .Returns(Enumerable.Empty<NationalDay>().ToList());
        
        var vacationDaysCalculatorService = new VacationDaysCalculatorService(nationalDaysService.Object);
        
        var requestedVacationIntervals = new List<VacationRequestInterval>
        {
            new VacationRequestInterval
            {
                Id = Guid.NewGuid(),
                StartDate = DateTimeOffset.UtcNow,
                EndDate = DateTimeOffset.UtcNow.AddDays(2)
            },
            new VacationRequestInterval
            {
                Id = Guid.NewGuid(),
                StartDate = DateTimeOffset.UtcNow.AddDays(3),
                EndDate = DateTimeOffset.UtcNow.AddDays(5)
            }
        };
        
        var vacationDaysCount = vacationDaysCalculatorService.CalculateVacationDaysForRequestedIntervals(requestedVacationIntervals);
        
        Assert.AreEqual(4, vacationDaysCount);
    }
    
    [Test]
    public void Given_VacationDaysCalculatorService_When_CalculateVacationDaysForRequestedIntervalsIsCalledWithNationalDays_ShouldReturnCorrectVacationDaysCount()
    {
        var nationalDaysService = new Mock<INationalDaysService>();
        
        nationalDaysService.Setup(x => x.GetNationalDays(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
            .Returns(new List<NationalDay>
            {
                new NationalDay
                {
                    Date = DateTimeOffset.UtcNow.AddDays(1),
                    Name = "National Day"
                }
            });
        
        var vacationDaysCalculatorService = new VacationDaysCalculatorService(nationalDaysService.Object);
        
        var requestedVacationIntervals = new List<VacationRequestInterval>
        {
            new VacationRequestInterval
            {
                Id = Guid.NewGuid(),
                StartDate = DateTimeOffset.UtcNow,
                EndDate = DateTimeOffset.UtcNow.AddDays(2)
            },
            new VacationRequestInterval
            {
                Id = Guid.NewGuid(),
                StartDate = DateTimeOffset.UtcNow.AddDays(3),
                EndDate = DateTimeOffset.UtcNow.AddDays(5)
            }
        };
        
        var vacationDaysCount = vacationDaysCalculatorService.CalculateVacationDaysForRequestedIntervals(requestedVacationIntervals);
        
        Assert.AreEqual(2, vacationDaysCount);
    }
}