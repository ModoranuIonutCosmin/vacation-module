using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VacationsModule.Domain.Entities;
using VacationsModule.WebApi.Controllers;

namespace VacationsModule.UnitTests.Auth_mock;

public class MockHttpContext {

    public static TController GetControllerWithAuthContext<TController>(ApplicationUser user, object[] args)
    where TController : BaseController<TController>
    {
        var identity = new ClaimsIdentity();

        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));

        var userPrincipal = new ClaimsPrincipal(identity);
        var controllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = userPrincipal } };


        // use reflection to instantiate a TController with args
        var controller = (TController)Activator.CreateInstance(typeof(TController), args);

        controller.ControllerContext = controllerContext;

        return controller;
    }
}