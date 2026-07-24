// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using Microsoft.AspNetCore.Mvc;
using Workflow.Web.Services.Processings;

namespace Workflow.Web.Controllers;

[ApiController]
public sealed class CoreUserController(
    ICoreUserProcessingService coreUserProcessingService)
    : ControllerBase
{
    [HttpGet("/Api/AppSecurity/User/Me()")]
    public IActionResult Get()
    {
        User user = coreUserProcessingService.GetUser();

        return Ok(value: user);
    }
}