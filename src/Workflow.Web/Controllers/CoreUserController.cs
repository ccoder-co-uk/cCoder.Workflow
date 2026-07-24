// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Workflow.Web.Controllers;

[ApiController]
public sealed class CoreUserController(ISecurityDbContextFactory securityDbContextFactory)
    : ControllerBase
{
    [HttpGet("/Api/AppSecurity/User/Me()")]
    public IActionResult Me()
    {
        using SecurityDbContext securityDbContext =
            securityDbContextFactory.CreateDbContext();

        SSOUser user = securityDbContext.GetCurrentUser();

        return Ok(value: new User
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Email = user.Email,
            DefaultCultureId = string.Empty,
            IsActive = true
        });
    }
}