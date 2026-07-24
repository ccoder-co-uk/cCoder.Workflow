// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Processings;

namespace cCoder.Workflow.Services.Orchestrations;

internal sealed partial class WorkflowEventOrchestrationService(IWorkflowEventProcessingService processingService, IWorkflowEventEventProcessingService eventService) : IWorkflowEventOrchestrationService
{
    public WorkflowEvent Get(Guid workflowEventId) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [workflowEventId]); return ExecuteGet(workflowEventId: workflowEventId); });

    private WorkflowEvent ExecuteGet(Guid workflowEventId)
    {
        return processingService.Get(workflowEventId: workflowEventId);
    }

    public IQueryable<WorkflowEvent> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<WorkflowEvent> ExecuteGetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask<WorkflowEvent> AddAsync(WorkflowEvent entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); return await ExecuteAddAsync(entity: entity); }, isValueTask: true);

    private async ValueTask<WorkflowEvent> ExecuteAddAsync(WorkflowEvent entity)
    {
        WorkflowEvent result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseWorkflowEventAddEventAsync(entity: result);
        return result;
    }

    public ValueTask<WorkflowEvent> UpdateAsync(WorkflowEvent entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); return await ExecuteUpdateAsync(entity: entity); }, isValueTask: true);

    private async ValueTask<WorkflowEvent> ExecuteUpdateAsync(WorkflowEvent entity)
    {
        WorkflowEvent result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseWorkflowEventUpdateEventAsync(entity: result);
        return result;
    }

    public ValueTask DeleteAsync(Guid workflowEventId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [workflowEventId]); await ExecuteDeleteAsync(workflowEventId: workflowEventId); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAsync(Guid workflowEventId)
    {
        WorkflowEvent entity = processingService.Get(workflowEventId: workflowEventId);
        await eventService.RaiseWorkflowEventDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(workflowEventId: workflowEventId);
    }

    public ValueTask<IEnumerable<Result<WorkflowEvent>>> AddOrUpdate(IEnumerable<WorkflowEvent> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); return await ExecuteAddOrUpdate(items: items); }, isValueTask: true);

    private ValueTask<IEnumerable<Result<WorkflowEvent>>> ExecuteAddOrUpdate(IEnumerable<WorkflowEvent> items)
    {
        return processingService.AddOrUpdate(items: items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<WorkflowEvent> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); await ExecuteDeleteAllAsync(items: items); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllAsync(IEnumerable<WorkflowEvent> items)
    {
        return processingService.DeleteAllAsync(items: items);
    }
}