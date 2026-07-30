// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using Microsoft.AspNetCore.Mvc;
using Workflow.Web.Services.Processings;

using Workflow.Web.Exposures;

namespace Workflow.Web.Controllers;

[ApiController]
public sealed class CoreUserController(
    ICoreUserManager coreUserProcessingService)
    : ControllerBase
{
    [HttpGet("/Api/AppSecurity/User/Me()")]
    public IActionResult Get()
    {
        User user = coreUserProcessingService.GetUser();

        return Ok(value: user);
    }
}