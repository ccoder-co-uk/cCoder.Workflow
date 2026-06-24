using System.Security;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations;
using cCoder.Workflow.Services.Processings;

namespace cCoder.Workflow.Services.Orchestrations;

internal class FlowDefinitionOrchestrationService(IFlowDefinitionProcessingService processingService, IFlowDefinitionEventProcessingService eventService, IFlowDefinitionService flowDefinitionService, IFlowInstanceDataOrchestrationService flowInstanceDataOrchestrationService, IAuthorizationBroker authorizationBroker, IJsonBroker jsonBroker) : IFlowDefinitionOrchestrationService
{
    public FlowDefinition Get(Guid id)
    {
        return processingService.Get(id);
    }

    public IQueryable<FlowDefinition> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters);
    }

    public async ValueTask<FlowDefinition> AddAsync(FlowDefinition entity)
    {
        FlowDefinition result = await processingService.AddAsync(entity);
        await eventService.RaiseFlowDefinitionAddEventAsync(result);
        return result;
    }

    public async ValueTask<FlowDefinition> UpdateAsync(FlowDefinition entity)
    {
        FlowDefinition result = await processingService.UpdateAsync(entity);
        await eventService.RaiseFlowDefinitionUpdateEventAsync(result);
        return result;
    }

    public ValueTask<Guid> QueueAsync(Guid id, string args)
    {
        FlowDefinition flowDefinition = flowDefinitionService.Get(id);
        authorizationBroker.Authorize(flowDefinition?.AppId, "flowdefinition_execute");
        string caller = authorizationBroker.GetCurrentUser()?.Id;
        return QueueAsync(flowDefinition, caller, args);
    }

    public ValueTask<Guid> QueueAsync(Guid id, string asUserId, string args)
    {
        FlowDefinition flowDefinition = flowDefinitionService.Get(id);
        authorizationBroker.Authorize(asUserId, flowDefinition?.AppId, "flowdefinition_execute");
        return QueueAsync(flowDefinition, asUserId, args);
    }

    public async ValueTask<Guid> QueueAsync(Guid id, cCoder.Data.Models.Security.User asUser, string args)
    {
        FlowDefinition flowDefinition = flowDefinitionService.Get(id);
        FlowInstanceData flowInstance = CreateFlowInstanceData(flowDefinition, asUser?.Id, args);
        return (await flowInstanceDataOrchestrationService.AddQueuedAsync(flowInstance)).Id;
    }

    private async ValueTask<Guid> QueueAsync(FlowDefinition flowDefinition, string caller, string args)
    {
        FlowInstanceData flowInstance = CreateFlowInstanceData(flowDefinition, caller, args);
        return (await flowInstanceDataOrchestrationService.AddQueuedAsync(flowInstance)).Id;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        FlowDefinition entity = processingService.GetAll(ignoreFilters: true).FirstOrDefault(item => item.Id == id);

        if (entity is null)
            return;

        await eventService.RaiseFlowDefinitionDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        FlowDefinition[] flows = [.. processingService.GetAll(ignoreFilters: true).Where(item => item.AppId == appId)];

        foreach (FlowDefinition flow in flows)
            await DeleteAsync(flow.Id);
    }

    public ValueTask<IEnumerable<Result<FlowDefinition>>> AddOrUpdate(IEnumerable<FlowDefinition> items)
    {
        return processingService.AddOrUpdate(items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<FlowDefinition> items)
    {
        return processingService.DeleteAllAsync(items);
    }

    private FlowInstanceData CreateFlowInstanceData(FlowDefinition flowDefinition, string caller, string args)
    {
        if (flowDefinition == null)
            throw new SecurityException("Access Denied!");

        if (string.IsNullOrWhiteSpace(caller))
            throw new SecurityException("Access Denied!");

        return FlowInstanceDataFactory.Create(flowDefinition, caller, args, jsonBroker);
    }
}
