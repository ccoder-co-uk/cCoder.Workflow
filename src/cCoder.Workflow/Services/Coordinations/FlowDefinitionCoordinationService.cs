using cCoder.Workflow.Api.OData;
using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Orchestrations;


namespace cCoder.Workflow.Services.Coordinations;

internal class FlowDefinitionCoordinationService(
    IFlowInstanceDataOrchestrationService flowInstanceDataOrchestrationService
) : IFlowDefinitionCoordinationService
{
    public async ValueTask HandleFlowDefinitionAddAsync(FlowDefinition flowDefinition)
    {
        if (flowDefinition.Instances == null || !flowDefinition.Instances.Any())
            return;

        await flowInstanceDataOrchestrationService.AddOrUpdate(flowDefinition.Instances);
    }

    public async ValueTask HandleFlowDefinitionUpdateAsync(FlowDefinition flowDefinition)
    {
        if (flowDefinition.Instances == null || !flowDefinition.Instances.Any())
            return;

        await flowInstanceDataOrchestrationService.AddOrUpdate(flowDefinition.Instances);
    }

    public async ValueTask HandleFlowDefinitionDeleteAsync(FlowDefinition flowDefinition)
    {
        IEnumerable<FlowInstanceData> instancesToDelete = flowInstanceDataOrchestrationService
            .GetAll(true)
            .Where(instance => instance.FlowDefinitionId == flowDefinition.Id)
            .ToArray();

        await flowInstanceDataOrchestrationService.DeleteAllAsync(instancesToDelete);
    }
}

