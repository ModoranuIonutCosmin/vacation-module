using System.ComponentModel.DataAnnotations;
using VacationsModule.Domain.Datamodels;

namespace VacationsModule.Application.DTOs.Auth;

public class RegisterEmployeeDataModelRequest
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public DateTimeOffset EmploymentDate { get; set; }

    public string? Department { get; set; }
    
    public string? Position { get; set; }

}