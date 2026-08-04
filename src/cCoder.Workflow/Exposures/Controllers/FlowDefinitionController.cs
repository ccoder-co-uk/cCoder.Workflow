// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using cCoder.Workflow.Dependencies;
using cCoder.Workflow.Extensions.OData;
using cCoder.Workflow.Models.OData;
using cCoder.Workflow.Models;
using cCoder.Data.Models.Workflow;
using cCoder.Security.Models.Configurations;
using cCoder.Workflow.Services.Aggregations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace cCoder.Workflow.Exposures.Controllers;

public partial class FlowDefinitionController(
    IFlowDefinitionManager service,
    ISSOAuthInfo authInfo) : ODataController
{
    [HttpGet]
    public IActionResult GetMetadata()
    {
        try
        {
            bool isExtendedMetaRequest = Request.Query["extend"] == "true";

            return isExtendedMetaRequest
                ? Ok(
    value: new cCoder.Workflow.Brokers.OData.WorkflowModelBroker()
                        .Build()
                        .EDMModel.GetExtendedMetadataForType(context: "Workflow", type: typeof(FlowDefinition))
                )
                : Ok(value: typeof(FlowDefinition).CreateMetadataContainer(isEntity: true, hasEndpoint: true));
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

    [HttpGet]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 5,
        MaxExpansionDepth = 5
    )]
    [ActionName("Get")]
    public IActionResult GetAll(ODataQueryOptions<FlowDefinition> queryOptions)
    {
        try
        {
            return Ok(value: service.GetAllFlowDefinitions());
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

    [HttpGet]
    [AllowAnonymous]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 3,
        MaxExpansionDepth = 3
    )]
    public IActionResult Get([FromRoute] Guid key)
    {
        try
        {
            FlowDefinition result = service.GetFlowDefinition(flowDefinitionId: key);
            return result is null ? NotFound() : Ok(value: result);
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

    [HttpPost]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 5,
        MaxExpansionDepth = 5
    )]
    public async Task<IActionResult> Post([FromBody] FlowDefinition newEntity)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return new cCoder.Workflow.Models.OData.BadRequestResult(ModelState);
            }

            return StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: await service.AddFlowDefinitionAsync(newEntity: newEntity));
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

    [HttpPut]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 5,
        MaxExpansionDepth = 5
    )]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] FlowDefinition updatedEntity)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return new cCoder.Workflow.Models.OData.BadRequestResult(ModelState);
            }

            return Ok(value: await service.UpdateFlowDefinitionAsync(updatedEntity: updatedEntity));
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

    [AcceptVerbs("PATCH", "MERGE")]
    [ActionName("Patch")]
    public async Task<IActionResult> Put([FromRoute] Guid key, Delta<FlowDefinition> updatedDelta)
    {
        try
        {
            FlowDefinition originalEntity = service.GetFlowDefinition(flowDefinitionId: key);

            if (originalEntity == null)
            {
                return NotFound();
            }

            updatedDelta.Patch(original: originalEntity);
            return Ok(value: await service.UpdateFlowDefinitionAsync(updatedEntity: originalEntity));
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

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        try
        {
            await service.DeleteFlowDefinitionAsync(flowDefinitionId: key);
            return NoContent();
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

    [HttpPost]
    [ActionName("Execute")]
    public async Task<IActionResult> PostAsync([FromRoute] Guid key)
    {
        try
        {
            string requestBody = await ReadRequestBodyAsync();
            string asUserId = authInfo.SSOUserId;
            return Ok(value: await service.QueueFlowDefinitionAsync(flowDefinitionId: key, asUserId: asUserId, args: requestBody));
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

    [HttpPost]
    [ActionName("ExecuteScript")]
    public async Task<IActionResult> PostScript()
    {
        try
        {
            string script = await ReadRequestBodyAsync();
            return Ok(value: await service.ExecuteScriptAsync(script: script));
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

    private async ValueTask<string> ReadRequestBodyAsync()
    {
        using WorkflowStreamDependency content = new();
        await Request.Body.CopyToAsync(destination: content);
        return Encoding.UTF8.GetString(bytes: content.ToArray());
    }
}