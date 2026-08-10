// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers.Loggings;
using cCoder.Data.Models.Security;
using Microsoft.AspNetCore.Mvc;
using Workflow.Web.Services.Processings;

using Workflow.Web.Exposures;

namespace Workflow.Web.Controllers;

[ApiController]
public sealed class CoreUserController(
    ICoreUserManager coreUserProcessingService,
    ILoggingBroker loggingBroker)
    : ControllerBase
{
    [HttpGet("/Api/AppSecurity/User/Me()")]
    public IActionResult Get()
    {
        try
        {
            User user = coreUserProcessingService.GetUser();

            return Ok(value: user);
        }
        catch (cCoder.Workflow.Models.Exceptions.WorkflowValidationException exception)
        {
            loggingBroker.LogError(exception: exception, message: "User validation failed.");

            return BadRequest(error: "The workflow request is invalid.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogError(exception: exception, message: "User authorization failed.");

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(exception: exception, message: "User request failed.");

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}