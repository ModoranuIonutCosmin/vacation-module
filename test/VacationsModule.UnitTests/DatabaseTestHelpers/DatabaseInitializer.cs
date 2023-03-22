using VacationsModule.Domain.Datamodels;
using VacationsModule.Domain.Entities;
using VacationsModule.Infrastructure.Data_Access;

namespace VacationsModule.UnitTests.DatabaseTestHelpers;

public class DatabaseInitializer
{
    public static void Initialize(VacationRequestsDBContext context)
    {
        if (context.Users.Any())
        {
            return;
        }
        
        Seed(context);
    }
    
    private static void Seed(VacationRequestsDBContext context)
    {
        
        var guidEmployee1 = Guid.Parse("95f8e50e-32e6-4f3e-b229-86c89089963c");
        var employee1 = new Employee()
        {
            Id = guidEmployee1,
            Department = "IT",
            Position = "Meserias",
            UserName = "markrobber1990",
            AccessFailedCount = 0,
            ConcurrencyStamp = "",
            Email = "markrobbers01@gmail.com",
            EmailConfirmed = true,
            EmploymentDate = DateTimeOffset.UtcNow,
            FirstName = "Mark",
            LastName = "Robbers",
            LockoutEnd = DateTimeOffset.UtcNow,
            LockoutEnabled = false,
            NormalizedEmail = "markrobbers01@gmail.com".ToUpper()
        };
        
        //Generate a few vacationRequests
        var guidVacationRequest1 = Guid.Parse("3b5ff972-894d-4785-8181-8a76264aa11f");
        var vacationRequest1 = new VacationRequest()
        {
            Id = guidVacationRequest1,
            Employee = employee1,
            Comments = new List<VacationRequestComment>(),
            CreatedAt = DateTimeOffset.UtcNow,
            Description = "",
            Status = VacationRequestStatus.Pending,
            VacationIntervals = new()
            {
                new VacationRequestInterval()
                {
                    Id = Guid.NewGuid(),
                    StartDate = DateTimeOffset.UtcNow,
                    EndDate = DateTimeOffset.UtcNow.AddYears(1)
                }
            },
        };

        context.Users.Add(employee1);
        context.VacationRequests.Add(vacationRequest1);

        context.SaveChanges();
    }

}