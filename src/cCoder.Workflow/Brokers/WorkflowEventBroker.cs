// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Data.Brokers;

internal sealed class WorkflowEventBroker(ICoreContextFactory coreContextFactory)
    : IWorkflowEventBroker
{

    public IQueryable<WorkflowEvent> SelectAllWorkflowEvents() =>
        coreContextFactory.CreateCoreContext().WorflowEvents;

    public IQueryable<WorkflowEvent> SelectAllWorkflowEventsIgnoringQueryFilters() =>
        coreContextFactory.CreateCoreContext()
            .WorflowEvents
            .IgnoreQueryFilters();

    public async ValueTask<WorkflowEvent> AddWorkflowEventAsync(WorkflowEvent newEntity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        WorkflowEvent result = (await coreDataContext.WorflowEvents.AddAsync(entity: newEntity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<WorkflowEvent> UpdateWorkflowEventAsync(WorkflowEvent updatedEntity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        WorkflowEvent result = coreDataContext.WorflowEvents.Update(entity: updatedEntity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteWorkflowEventAsync(WorkflowEvent deletedEntity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.WorflowEvents.Remove(entity: deletedEntity);
        return await coreDataContext.SaveChangesAsync();
    }

    public int? SelectAppId(WorkflowEvent entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.FlowDefinitions

            .Where(predicate: flowDefinition => flowDefinition.Id == entity.FlowId)
            .Select(selector: flowDefinition => (int?)flowDefinition.AppId)
            .FirstOrDefault();

    }
}