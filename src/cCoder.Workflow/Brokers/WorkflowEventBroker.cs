// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Data.Brokers;

public class WorkflowEventBroker(ICoreContextFactory coreContextFactory) 
    : IWorkflowEventBroker
{

    public IQueryable<WorkflowEvent> GetAllWorkflowEvents(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return ignoreFilters
            ? coreDataContext.WorflowEvents.IgnoreQueryFilters()
            : coreDataContext.WorflowEvents;
    }

    public async ValueTask<WorkflowEvent> AddWorkflowEventAsync(WorkflowEvent entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        WorkflowEvent result = (await coreDataContext.WorflowEvents.AddAsync(entity:entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<WorkflowEvent> UpdateWorkflowEventAsync(WorkflowEvent entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        WorkflowEvent result = coreDataContext.WorflowEvents.Update(entity:entity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteWorkflowEventAsync(WorkflowEvent entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.WorflowEvents.Remove(entity:entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllWorkflowEventsAsync(IEnumerable<WorkflowEvent> items)
    {
        if (items == null || !items.Any())
            return;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.WorflowEvents.RemoveRange(entities:items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(WorkflowEvent entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return coreDataContext.FlowDefinitions

            .Where(predicate:flowDefinition => flowDefinition.Id == entity.FlowId)
            .Select(selector:flowDefinition => (int?)flowDefinition.AppId)
            .FirstOrDefault();

    }
}