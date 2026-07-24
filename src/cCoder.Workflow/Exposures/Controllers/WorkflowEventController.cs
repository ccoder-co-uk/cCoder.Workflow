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

public partial class WorkflowEventController : ODataController
{
    private readonly IWorkflowEventOrchestrationService service;

    public WorkflowEventController(IWorkflowEventOrchestrationService service)
    {
        this.service = service;
    }

    [HttpGet]
    public IActionResult GetMetadata()
    {
        bool isExtendedMetaRequest = Request.Query["extend"] == "true";

        return isExtendedMetaRequest
            ? Ok(
value: new cCoder.Workflow.Api.OData.WorkflowModelBuilder()
                    .Build()
                    .EDMModel.GetExtendedMetadataForType(context: "Workflow", type: typeof(WorkflowEvent))
            )
            : Ok(value: new MetadataContainer(typeof(WorkflowEvent), true, true));
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
    public IActionResult GetAll(ODataQueryOptions<WorkflowEvent> queryOptions) =>
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
            IQueryable<WorkflowEvent> result = service.GetAll()
                .Where(predicate: workflowEvent => workflowEvent.Id == key);

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
    public async Task<IActionResult> Post([FromBody] WorkflowEvent newEntity)
    {
        if (!ModelState.IsValid)
        {
            return new cCoder.Workflow.Api.OData.BadRequestResult(ModelState);
        }

        return Ok(value: await service.AddWorkflowEventAsync(newEntity: newEntity));
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
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] WorkflowEvent updatedEntity)
    {
        if (!ModelState.IsValid)
        {
            return new cCoder.Workflow.Api.OData.BadRequestResult(ModelState);
        }

        return Ok(value: await service.UpdateWorkflowEventAsync(updatedEntity: updatedEntity));
    }

    [AcceptVerbs("PATCH", "MERGE")]
    public async Task<IActionResult> Put([FromRoute] Guid key, Delta<WorkflowEvent> updatedDelta)
    {
        WorkflowEvent originalEntity = service.Get(workflowEventId: key);

        if (originalEntity == null)
        {
            return NotFound();
        }

        updatedDelta.Patch(original: originalEntity);
        return Ok(value: await service.UpdateWorkflowEventAsync(updatedEntity: originalEntity));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        await service.DeleteAsync(workflowEventId: key);
        return Ok();
    }
}