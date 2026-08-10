// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers.Loggings;
using Microsoft.AspNetCore.Mvc;
using Workflow.HostedServices.Services.Processings;

using Workflow.HostedServices.Exposures;

namespace Workflow.HostedServices.Controllers;

[ApiController]
public sealed class HomeController(
    IHomeManager homeProcessingService,
    ILoggingBroker loggingBroker)
    : ControllerBase
{
    [HttpGet("/")]
    public IActionResult Get()
    {
        try
        {
            return Content( content: homeProcessingService.GetHome(), contentType: "text/plain");
        }
        catch (cCoder.Workflow.Models.Exceptions.WorkflowValidationException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Home validation failed.");

            return BadRequest(error: "The workflow request is invalid.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogError(exception: exception, message: "Home authorization failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "Home request failed.");

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}