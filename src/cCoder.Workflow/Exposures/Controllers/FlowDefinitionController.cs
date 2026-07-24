// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using cCoder.Workflow.Dependencies.OData;
using cCoder.Workflow.Models;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Aggregations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace cCoder.Workflow.Exposures.Controllers;

public partial class FlowDefinitionController(IFlowDefinitionAggregationService service) : ODataController
{
    [HttpGet]
    public IActionResult GetMetadata()
    {
        bool isExtendedMetaRequest = Request.Query["extend"] == "true";

        return isExtendedMetaRequest
            ? Ok(
value: new cCoder.Workflow.Dependencies.OData.WorkflowModelBuilder()
                    .Build()
                    .EDMModel.GetExtendedMetadataForType(context: "Workflow", type: typeof(FlowDefinition))
            )
            : Ok(value: new MetadataContainer(typeof(FlowDefinition), true, true));
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
    public IActionResult GetAll(ODataQueryOptions<FlowDefinition> queryOptions) =>
        Ok(value: service.GetAllFlowDefinitions());

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
        catch (System.Security.SecurityException)
        {
            return NotFound();
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
        if (!ModelState.IsValid)
        {
            return new cCoder.Workflow.Dependencies.OData.BadRequestResult(ModelState);
        }

        return Ok(value: await service.AddFlowDefinitionAsync(newEntity: newEntity));
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
        if (!ModelState.IsValid)
        {
            return new cCoder.Workflow.Dependencies.OData.BadRequestResult(ModelState);
        }

        return Ok(value: await service.UpdateFlowDefinitionAsync(updatedEntity: updatedEntity));
    }

    [AcceptVerbs("PATCH", "MERGE")]
    [ActionName("Patch")]
    public async Task<IActionResult> Put([FromRoute] Guid key, Delta<FlowDefinition> updatedDelta)
    {
        FlowDefinition originalEntity = service.GetFlowDefinition(flowDefinitionId: key);

        if (originalEntity == null)
        {
            return NotFound();
        }

        updatedDelta.Patch(original: originalEntity);
        return Ok(value: await service.UpdateFlowDefinitionAsync(updatedEntity: originalEntity));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        await service.DeleteFlowDefinitionAsync(flowDefinitionId: key);
        return Ok();
    }

    [HttpPost]
    [ActionName("Execute")]
    public async Task<IActionResult> PostAsync([FromRoute] Guid key)
    {
        using StreamReader reader = new(Request.Body, Encoding.UTF8);
        string asUserId = User?.Identity?.Name;
        return Ok(value: await service.QueueFlowDefinitionAsync(flowDefinitionId: key, asUserId: asUserId, args: await reader.ReadToEndAsync()));
    }

    [HttpPost]
    [ActionName("ExecuteScript")]
    public async Task<IActionResult> PostScript()
    {
        string script = await new StreamReader(Request.Body).ReadToEndAsync();
        return Ok(value: await service.ExecuteScriptAsync(script: script));
    }

    [HttpGet]
    [ActionName("KnownActivityTypes")]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.All,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 6,
        MaxExpansionDepth = 6
    )]
    public IActionResult GetKnownActivityTypes()
    {
        return Ok(value: service.GetKnownActivityTypes());
    }

    [AllowAnonymous]
    [HttpGet]
    [ActionName("KnownSystemTypes")]
    public IActionResult GetKnownSystemTypes()
    {
        return Ok(value: service.GetKnownSystemTypes());
    }
}