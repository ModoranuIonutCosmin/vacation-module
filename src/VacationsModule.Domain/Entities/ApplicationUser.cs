using Microsoft.AspNetCore.Identity;
using VacationsModule.Domain.Seedwork;

namespace VacationsModule.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>, IAggregateRoot
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
}