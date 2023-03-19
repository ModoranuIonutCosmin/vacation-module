using Microsoft.AspNetCore.Mvc;

namespace VacationsModule.WebApi.ApiResponses.ExtensionMethods;

public static class ResponseHelpers
{
    public static ObjectResult Forbidden<T> (this ControllerBase controllerBase, T value)
    {
        return new ObjectResult(value)
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
    }
    
    public static ObjectResult BadRequest<T> (this ControllerBase controllerBase, T value)
    {
        return new ObjectResult(value)
        {
            StatusCode = StatusCodes.Status400BadRequest
        };
    }
}