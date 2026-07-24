// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Workflow.Web.Controllers;

[ApiController]
public sealed class CoreAppController(ICoreContextFactory coreContextFactory) : ControllerBase
{
    [HttpGet("/Api/ContentManagement/App({key:int})")]
    public async Task<IActionResult> Get([FromRoute] int key)
    {
        await using CoreDataContext core = coreContextFactory.CreateCoreContext();

        App app = await core.Set<App>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(predicate: found => found.Id == key);

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