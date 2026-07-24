// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Processings;

namespace cCoder.Workflow.Services.Orchestrations;

internal class WorkflowEventOrchestrationService(IWorkflowEventProcessingService processingService, IWorkflowEventEventProcessingService eventService) : IWorkflowEventOrchestrationService
{
    public WorkflowEvent Get(Guid workflowEventId)
    {
        return processingService.Get(workflowEventId: workflowEventId);
    }

    public IQueryable<WorkflowEvent> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters: ignoreFilters);
    }

    public async ValueTask<WorkflowEvent> AddAsync(WorkflowEvent entity)
    {
        WorkflowEvent result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseWorkflowEventAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<WorkflowEvent> UpdateAsync(WorkflowEvent entity)
    {
        WorkflowEvent result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseWorkflowEventUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(Guid workflowEventId)
    {
        WorkflowEvent entity = processingService.Get(workflowEventId: workflowEventId);
        await eventService.RaiseWorkflowEventDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(workflowEventId: workflowEventId);
    }

    public ValueTask<IEnumerable<Result<WorkflowEvent>>> AddOrUpdate(IEnumerable<WorkflowEvent> items)
    {
        return processingService.AddOrUpdate(items: items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<WorkflowEvent> items)
    {
        return processingService.DeleteAllAsync(items: items);
    }
}