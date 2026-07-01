using cCoder.Data.Models.Security;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Workflow.Web.Controllers;

[ApiController]
public sealed class CoreUserController(IAuthenticationOrchestrationService authenticationOrchestrationService)
    : ControllerBase
{
    [HttpGet("/Api/Core/User/Me()")]
    public IActionResult Me()
    {
        SSOUser user = authenticationOrchestrationService.Me();

        return Ok(new User
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Email = user.Email,
            DefaultCultureId = string.Empty,
            IsActive = true
        });
    }
}
