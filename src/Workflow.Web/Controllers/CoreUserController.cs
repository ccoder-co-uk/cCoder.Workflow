using cCoder.Data.Models.Security;
using cCoder.Security.Exposures;
using cCoder.Security.Objects.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Workflow.Web.Controllers;

[ApiController]
public sealed class CoreUserController(IAccountManager accountManager) : ControllerBase
{
    [HttpGet("/Api/Core/User/Me()")]
    public IActionResult Me()
    {
        SSOUser user = accountManager.Me();

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
