// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers.Loggings;
using Microsoft.AspNetCore.Mvc;
using Workflow.Web.Services.Processings;

using Workflow.Web.Exposures;

namespace Workflow.Web.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class HealthController(
    IHealthManager healthProcessingService,
    ILoggingBroker loggingBroker)
    : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            return Content( content: healthProcessingService.GetHealth(), contentType: "text/plain");
        }
        catch (cCoder.Workflow.Models.Exceptions.WorkflowValidationException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Health validation failed.");

            return BadRequest(error: "The workflow request is invalid.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Health authorization failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Health request failed.");

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}