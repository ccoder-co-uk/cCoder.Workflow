// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using cCoder.Workflow.Api.OData;
using cCoder.Workflow.Models;
using cCoder.Data.Models.Workflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace cCoder.Workflow.Exposures.Controllers;

public partial class FlowDefinitionController(IFlowDefinitionControllerService service) : ODataController
{
    [HttpGet]
    public IActionResult GetMetadata()
    {
        bool isExtendedMetaRequest = Request.Query["extend"] == "true";

        return isExtendedMetaRequest
            ? Ok(
value: new cCoder.Workflow.Api.OData.WorkflowModelBuilder()
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
        Ok(value: service.GetAll());

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
            FlowDefinition result = service.Get(flowDefinitionId: key);
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
    public async Task<IActionResult> Post([FromBody] FlowDefinition entity)
    {
        if (!ModelState.IsValid)
        {
            return new cCoder.Workflow.Api.OData.BadRequestResult(ModelState);
        }

        return Ok(value: await service.AddAsync(entity: entity));
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
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] FlowDefinition entity)
    {
        if (!ModelState.IsValid)
        {
            return new cCoder.Workflow.Api.OData.BadRequestResult(ModelState);
        }

        return Ok(value: await service.UpdateAsync(entity: entity));
    }

    [AcceptVerbs("PATCH", "MERGE")]
    public async Task<IActionResult> Patch([FromRoute] Guid key, Delta<FlowDefinition> delta)
    {
        FlowDefinition originalEntity = service.Get(flowDefinitionId: key);
        if (originalEntity == null)
        {
            return NotFound();
        }

        delta.Patch(original: originalEntity);
        return Ok(value: await service.UpdateAsync(entity: originalEntity));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        await service.DeleteAsync(flowDefinitionId: key);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> ExecuteAsync([FromRoute] Guid key)
    {
        using StreamReader reader = new(Request.Body, Encoding.UTF8);
        string asUserId = User?.Identity?.Name;
        return Ok(value: await service.QueueAsync(flowDefinitionId: key, asUserId: asUserId, args: await reader.ReadToEndAsync()));
    }

    [HttpPost]
    public async Task<IActionResult> ExecuteScript()
    {
        string script = await new StreamReader(Request.Body).ReadToEndAsync();
        return Ok(value: await service.ExecuteScriptAsync(script: script));
    }

    [HttpGet]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.All,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 6,
        MaxExpansionDepth = 6
    )]
    public IActionResult KnownActivityTypes()
    {
        return Ok(value: service.GetKnownActivityTypes());
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult KnownSystemTypes()
    {
        return Ok(value: service.GetKnownSystemTypes());
    }
}