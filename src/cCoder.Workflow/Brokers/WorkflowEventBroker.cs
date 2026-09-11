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

    public WorkflowEvent[] SelectWorkflowEventSubscriptions(
        int appId,
        string eventContext) =>
        coreContextFactory.CreateCoreContext()
            .WorflowEvents
            .IgnoreQueryFilters()
            .Where(predicate: workflowEvent =>
                workflowEvent.Flow.AppId == appId &&
                workflowEvent.EventContext == eventContext)
            .Include(navigationPropertyPath: workflowEvent => workflowEvent.Flow)
            .Include(navigationPropertyPath: workflowEvent => workflowEvent.ExecuteAsUser)
                .ThenInclude(navigationPropertyPath: user => user.Roles)
                    .ThenInclude(navigationPropertyPath: userRole => userRole.Role)
            .ToArray();

    public async ValueTask<WorkflowEvent> AddWorkflowEventAsync(WorkflowEvent newWorkflowEvent)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        WorkflowEvent result = (await coreDataContext.WorflowEvents.AddAsync(entity: newWorkflowEvent)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<WorkflowEvent> UpdateWorkflowEventAsync(WorkflowEvent updatedWorkflowEvent)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        WorkflowEvent result = coreDataContext.WorflowEvents.Update(entity: updatedWorkflowEvent).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteWorkflowEventAsync(WorkflowEvent deletedWorkflowEvent)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.WorflowEvents.Remove(entity: deletedWorkflowEvent);
        return await coreDataContext.SaveChangesAsync();
    }

    public int? SelectAppId(WorkflowEvent workflowEvent)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.FlowDefinitions

            .Where(predicate: flowDefinition => flowDefinition.Id == workflowEvent.FlowId)
            .Select(selector: flowDefinition => (int?)flowDefinition.AppId)
            .FirstOrDefault();

    }
}