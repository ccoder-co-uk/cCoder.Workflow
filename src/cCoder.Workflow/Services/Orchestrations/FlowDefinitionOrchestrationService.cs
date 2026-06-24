using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Processings;

namespace cCoder.Workflow.Services.Orchestrations;

internal class FlowDefinitionOrchestrationService(
    IFlowDefinitionProcessingService processingService,
    IFlowDefinitionEventProcessingService eventService)
        : IFlowDefinitionOrchestrationService
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
}
