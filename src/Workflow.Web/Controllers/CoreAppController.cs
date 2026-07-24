// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using Microsoft.AspNetCore.Mvc;
using Workflow.Web.Services.Processings;

namespace Workflow.Web.Controllers;

[ApiController]
public sealed class CoreAppController(
    ICoreAppProcessingService coreAppProcessingService)
    : ControllerBase
{
    [HttpGet("/Api/ContentManagement/App({key:int})")]
    public async Task<IActionResult> Get([FromRoute] int key)
    {
        App app = await coreAppProcessingService.GetAppAsync(appId: key);

        if (app is null)
        {
            return NotFound();
        }

        return Ok(value: new
        {
            app.Id,
            app.DefaultCultureId,
            app.TenantId,
            app.Name,
            app.Domain,
            app.DefaultTheme,
            app.ConfigJson
        });
    }
}