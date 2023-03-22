using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VacationsModule.Domain.Entities;
using VacationsModule.WebApi.Controllers;

namespace VacationsModule.UnitTests.Auth_mock;

public class MockHttpContext {

    public static TController GetControllerWithAuthContext<TController>(ApplicationUser user)
    where TController: BaseController<TController>, new()
    {
        var identity = new ClaimsIdentity();
        
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        
        var userPrincipal = new ClaimsPrincipal(identity);
        var controllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = userPrincipal } };
        
        
        return new TController()
        {
            ControllerContext = controllerContext
        };
    }
}