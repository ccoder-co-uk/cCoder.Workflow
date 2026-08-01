// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Workflow.HostedServices.Services.Processings;

using Workflow.HostedServices.Exposures;

namespace Workflow.HostedServices.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class HealthController(
    IHealthManager healthProcessingService)
    : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            return Content( content: healthProcessingService.GetHealth(), contentType: "text/plain");
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