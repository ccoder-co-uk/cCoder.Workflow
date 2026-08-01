// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Services.Foundations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace cCoder.Workflow.Exposures.Controllers;

[Route("Api/Workflow/FlowDefinition")]
public sealed class FlowDefinitionMetadataController(
    IWorkflowMetadataTypeManager service)
    : ControllerBase
{
    [HttpGet("KnownActivityTypes()")]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.All,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 6,
        MaxExpansionDepth = 6)]
    public IActionResult GetKnownActivityTypes()
    {
        try
        {
            return Ok(value: service.GetKnownActivityTypes());
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

    [AllowAnonymous]
    [HttpGet("KnownSystemTypes()")]
    public IActionResult GetKnownSystemTypes()
    {
        try
        {
            return Ok(value: service.GetKnownSystemTypes());
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