using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace VacationsModule.WebApi.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class BaseController<TDerived> : ControllerBase
{
    protected readonly ILogger<TDerived> _logger;

    public BaseController(ILogger<TDerived> logger)
    {
        _logger = logger;
    }
}