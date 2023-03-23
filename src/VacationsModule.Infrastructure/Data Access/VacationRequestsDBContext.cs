using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VacationsModule.Domain.Datamodels;
using VacationsModule.Domain.Entities;
using VacationsModule.Domain.Seedwork;
using VacationsModule.Infrastructure.Data_Access.EntityConfig;

namespace VacationsModule.Infrastructure.Data_Access;

public class VacationRequestsDBContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    private readonly IMediator _mediator;

    // public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<VacationRequest> VacationRequests { get; set; }
    public DbSet<VacationRequestComment> Comments { get; set; }
    public DbSet<VacationRequestInterval> VacationIntervals { get; set; }

    public VacationRequestsDBContext(DbContextOptions<VacationRequestsDBContext> options,
        IMediator mediator) : base(options)
    {
        _mediator = mediator;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new VacationRequestEntityConfiguration());
        modelBuilder.ApplyConfiguration(new VacationRequestCommentEntityConfiguration());
        modelBuilder.ApplyConfiguration(new VacationRequestIntervalEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ApplicationUserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new EmployeeUserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ManagerUserEntityConfiguration());


        var managerRole = new ApplicationRole()
        {
            Id = Guid.Parse("2c5e174e-3b0e-446f-86af-483d56fd7210"),
            Name = "Manager",
            NormalizedName = "MANAGER".ToUpper()
        };

        var employeeRole = new ApplicationRole()
        {
            Id = Guid.Parse("c6f97274-d239-444a-b96e-6d5f3fa97a1d"),
            Name = RolesEnum.Employee.ToString(),
            NormalizedName = RolesEnum.Employee.ToString().ToUpper()
        };

        modelBuilder.Entity<ApplicationRole>().HasData(managerRole, employeeRole);

        var hasher = new PasswordHasher<ApplicationRole>();

        var mangerUserName = "manageruser01";
        modelBuilder.Entity<Manager>().HasData(
            new Manager
            {
                Id = Guid.Parse("8e445865-a24d-4543-a6c6-9443d048cdb9"), // primary key
                UserName =mangerUserName,
                NormalizedUserName = "MANAGERUSER01",
                Department = "IT",
                Position = "Developer",
                FirstName = "Mark",
                LastName = "Robbers",
                Email = "mark@kenr.com",
                NormalizedEmail = "MARK@KENR.COM",
                PasswordHash = hasher.HashPassword(null, "string123")
            }
        );


        //Seeding the relation between our user and role to AspNetUserRoles table
        modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(
            new IdentityUserRole<Guid>
            {
                RoleId = Guid.Parse("2c5e174e-3b0e-446f-86af-483d56fd7210"),
                UserId = Guid.Parse("8e445865-a24d-4543-a6c6-9443d048cdb9")
            }
        );
        
        modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(
            new IdentityUserRole<Guid>
            {
                RoleId = Guid.Parse("c6f97274-d239-444a-b96e-6d5f3fa97a1d"),
                UserId = Guid.Parse("8e445865-a24d-4543-a6c6-9443d048cdb9")
            }
        );
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var response = await base.SaveChangesAsync(cancellationToken);
        await _dispatchDomainEvents();
        return response;
    }

    private async Task _dispatchDomainEvents()
    {
        var domainEventEntities = ChangeTracker.Entries<Entity>()
            .Select(po => po.Entity)
            .Where(po => po.DomainEvents.Any())
            .ToArray();

        foreach (var entity in domainEventEntities)
        {
            var events = entity.DomainEvents.ToArray();
            entity.DomainEvents.Clear();
            foreach (var entityDomainEvent in events)
            {
                await _mediator.Publish(entityDomainEvent);
            }
        }
    }
}