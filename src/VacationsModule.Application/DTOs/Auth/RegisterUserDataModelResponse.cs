﻿namespace VacationsModule.Application.DTOs.Auth;

public class RegisterUserDataModelResponse
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public string Token { get; set; }
}