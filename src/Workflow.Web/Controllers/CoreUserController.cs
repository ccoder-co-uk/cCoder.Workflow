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
        try
        {
            User user = coreUserProcessingService.GetUser();

            return Ok(value: user);
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