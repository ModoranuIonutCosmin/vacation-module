using VacationsModule.Domain.Datamodels;

namespace VacationsModule.Application.DTOs.Auth;

public class RegisterUserDataModelRequest
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

}