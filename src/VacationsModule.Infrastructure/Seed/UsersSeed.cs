using Microsoft.AspNetCore.Identity;
using VacationsModule.Domain.Datamodels;
using VacationsModule.Domain.Entities;

namespace VacationsModule.Infrastructure.Seed;

public class UsersSeed
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public UsersSeed(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task Seed()
    {
        if (_roleManager.Roles.Any())
        {
            return;
        }
        
        var role1Manager = new ApplicationRole()
        {
            Name = RolesEnum.Manager.ToString(),
        };
        
        var role2Employee = new ApplicationRole()
        {
            Name = RolesEnum.Employee.ToString(),
        };
        
        await _roleManager.CreateAsync(role2Employee);
        await _roleManager.CreateAsync(role1Manager);

        string user1Password = "string123";
        string user2Password = "string123";
        var user1Employee = new Employee()
        {
            Department = "IT",
            Position = "Developer",
            FirstName = "John",
            LastName = "Doe",
            Email = "john@kenr.com",
            UserName = "johnnyEmp",
            PhoneNumber = "123456789"
        };
        
        var user2Manager = new Manager()
        {
            Department = "IT",
            Position = "Developer",
            FirstName = "Mark",
            LastName = "Robbers",
            Email = "mark@kenr.com",
            UserName = "markyMngr",
            PhoneNumber = "123456789"
        };
        
        await _userManager.CreateAsync(user1Employee, user1Password);
        await _userManager.AddToRoleAsync(user1Employee, role2Employee.Name);

        await _userManager.CreateAsync(user2Manager, user1Password);
        await _userManager.AddToRoleAsync(user2Manager, role1Manager.Name);
    }
}