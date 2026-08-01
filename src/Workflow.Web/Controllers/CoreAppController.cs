// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using Microsoft.AspNetCore.Mvc;
using Workflow.Web.Services.Processings;

using Workflow.Web.Exposures;

namespace Workflow.Web.Controllers;

[ApiController]
public sealed class CoreAppController(
    ICoreAppManager coreAppProcessingService)
    : ControllerBase
{
    [HttpGet("/Api/ContentManagement/App({key:int})")]
    public async Task<IActionResult> Get([FromRoute] int key)
    {
        try
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
        catch (cCoder.Workflow.Models.Exceptions.WorkflowValidationException)
        {
            return BadRequest(error: "The workflow request is invalid.");
        }
        catch (System.Security.SecurityException)
        {
            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception)
        {
            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}