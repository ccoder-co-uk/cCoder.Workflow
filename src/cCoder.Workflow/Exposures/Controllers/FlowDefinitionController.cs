using System.Net;
using System.Text;
using cCoder.Data;
using cCoder.Data.Extensions;
using cCoder.Workflow.Api.OData;
using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations;
using cCoder.Workflow.Services.Orchestrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;


namespace cCoder.Workflow.Exposures.Controllers;

public partial class FlowDefinitionController(
        IFlowDefinitionOrchestrationService service,
        IWorkflowMetadataTypeService metadataTypeService,
        Config config
    ) : ODataController
{
    [HttpGet]
    public IActionResult GetMetadata()
    {
        bool isExtendedMetaRequest = Request.Query["extend"] == "true";

        return isExtendedMetaRequest
            ? Ok(
                new cCoder.Workflow.Api.OData.WorkflowModelBuilder()
                    .Build()
                    .EDMModel.GetExtendedMetadataForType("Core", typeof(FlowDefinition))
            )
            : Ok(new MetadataContainer(typeof(FlowDefinition), true, true));
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
        Ok(service.GetAll());

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
            IQueryable<FlowDefinition> result = service.GetAll().Where(flowDefinition => flowDefinition.Id == key);
            return Ok(SingleResult.Create(result));
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
            return new cCoder.Workflow.Api.OData.BadRequestResult(ModelState);

        return Ok(await service.AddAsync(entity));
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
            return new cCoder.Workflow.Api.OData.BadRequestResult(ModelState);

        return Ok(await service.UpdateAsync(entity));
    }

    [AcceptVerbs("PATCH", "MERGE")]
    public async Task<IActionResult> Patch([FromRoute] Guid key, Delta<FlowDefinition> delta)
    {
        FlowDefinition originalEntity = service.Get(key);
        if (originalEntity == null)
            return NotFound();

        delta.Patch(originalEntity);
        return Ok(await service.UpdateAsync(originalEntity));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        await service.DeleteAsync(key);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> ExecuteAsync([FromRoute] Guid key)
    {
        using StreamReader reader = new(Request.Body, Encoding.UTF8);
        return Ok(await service.QueueAsync(key, await reader.ReadToEndAsync()));
    }

    [HttpPost]
    public async Task<IActionResult> ExecuteScript()
    {
        using HttpClient api = new(
            new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            }
        )
        {
            BaseAddress = new Uri(config.Services["Workflow"]),
            Timeout = TimeSpan.FromMinutes(10),
        };

        string script = await new StreamReader(Request.Body).ReadToEndAsync();
        HttpResponseMessage response = await api.PostAsync(
            "ExecuteScript",
            new StringContent(script, Encoding.UTF8, "text/plain")
        );
        return Ok(await response.Content.ReadAsStringAsync());
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
        return Ok(metadataTypeService.GetKnownActivityTypes());
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult KnownSystemTypes()
    {
        return Ok(metadataTypeService.GetKnownSystemTypes());
    }
}

















