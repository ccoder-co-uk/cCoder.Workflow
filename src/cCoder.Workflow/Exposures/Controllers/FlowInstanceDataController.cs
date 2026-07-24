// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Api.OData;
using cCoder.Workflow.Models;
using cCoder.Data.Extensions;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Orchestrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;


namespace cCoder.Workflow.Exposures.Controllers;

public partial class FlowInstanceDataController : ODataController
{
    protected IFlowInstanceDataOrchestrationService Service { get; }

    public FlowInstanceDataController(IFlowInstanceDataOrchestrationService service)
    {
        Service = service;
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    [HttpPut]
    [EnableQuery(
        AllowedArithmeticOperators = AllowedArithmeticOperators.All,
        AllowedFunctions = AllowedFunctions.AllFunctions,
        AllowedLogicalOperators = AllowedLogicalOperators.All,
        AllowedQueryOptions = AllowedQueryOptions.All,
        MaxAnyAllExpressionDepth = 3,
        MaxExpansionDepth = 3
    )]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] FlowInstanceData entity)
    {
        if (!ModelState.IsValid)
        {
            return new cCoder.Workflow.Api.OData.BadRequestResult(ModelState);
        }

        entity.Id = key;
        await Service.UpdateAsync(entity: entity);
        return NoContent();
    }

    [HttpGet]
    public IActionResult GetMetadata()
    {
        bool isExtendedMetaRequest = Request.Query["extend"] == "true";

        return isExtendedMetaRequest
            ? Ok(
value: new cCoder.Workflow.Api.OData.WorkflowModelBuilder()
                    .Build()
                    .EDMModel.GetExtendedMetadataForType(context: "Workflow", type: typeof(FlowInstanceData))
            )
            : Ok(value: new MetadataContainer(typeof(FlowInstanceData), true, true));
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
    public IActionResult GetAll(ODataQueryOptions<FlowInstanceData> queryOptions) =>
        Ok(value: Service.GetAll());

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
            IQueryable<FlowInstanceData> result = Service.GetAll()
                .Where(predicate: flowInstanceData => flowInstanceData.Id == key);
            return Ok(value: SingleResult.Create(queryable: result));
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
    public async Task<IActionResult> Post([FromBody] FlowInstanceData entity)
    {
        if (!ModelState.IsValid)
        {
            return new cCoder.Workflow.Api.OData.BadRequestResult(ModelState);
        }

        return Ok(value: await Service.AddAsync(entity: entity));
    }

    [AcceptVerbs("PATCH", "MERGE")]
    public async Task<IActionResult> Patch([FromRoute] Guid key, Delta<FlowInstanceData> delta)
    {
        FlowInstanceData originalEntity = Service.Get(flowInstanceDataId: key);
        if (originalEntity == null)
        {
            return NotFound();
        }

        delta.Patch(original: originalEntity);
        return Ok(value: await Service.UpdateAsync(entity: originalEntity));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        await Service.DeleteAsync(flowInstanceDataId: key);
        return Ok();
    }
}