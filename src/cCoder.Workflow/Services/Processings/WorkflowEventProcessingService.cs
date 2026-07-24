// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Foundations;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Workflow.Services.Processings;

internal class WorkflowEventProcessingService(
    IWorkflowEventService service,
    IAuthorizationBroker authorizationBroker)
        : IWorkflowEventProcessingService
{
    public WorkflowEvent Get(Guid workflowEventId)
    {
        return service.Get(workflowEventId: workflowEventId);
    }

    public IQueryable<WorkflowEvent> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask<WorkflowEvent[]> GetSubscriptionsAsync(int appId, string eventContext)
    {
        WorkflowEvent[] subscriptions = service
            .GetAll(ignoreFilters: true)
            .Where(predicate: item => item.Flow.AppId == appId && item.EventContext == eventContext)
            .Include(navigationPropertyPath: item => item.Flow)
            .Include(navigationPropertyPath: item => item.ExecuteAsUser)
                .ThenInclude(navigationPropertyPath: user => user.Roles)
                    .ThenInclude(navigationPropertyPath: userRole => userRole.Role)
            .ToArray();

        return ValueTask.FromResult(result: subscriptions);
    }

    public ValueTask<WorkflowEvent> AddAsync(WorkflowEvent entity)
    {
        SecurityCheckEvent(workflowEvent: entity);
        return service.AddAsync(workflowEvent: entity);
    }

    public ValueTask<WorkflowEvent> UpdateAsync(WorkflowEvent entity)
    {
        SecurityCheckEvent(workflowEvent: entity);
        return service.UpdateAsync(workflowEvent: entity);
    }

    public ValueTask DeleteAsync(Guid workflowEventId)
    {
        return service.DeleteAsync(workflowEventId: workflowEventId);
    }

    public async ValueTask<IEnumerable<Result<WorkflowEvent>>> AddOrUpdate(IEnumerable<WorkflowEvent> items)
    {
        List<Result<WorkflowEvent>> results = new List<Result<WorkflowEvent>>();

        foreach (WorkflowEvent item in items)
        {
            try
            {
                WorkflowEvent savedItem =
                    item.Id == Guid.Empty
                        ? await AddAsync(entity: item)
                        : await UpdateAsync(entity: item);

                results.Add(item: new Result<WorkflowEvent>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<WorkflowEvent>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<WorkflowEvent> items)
    {
        foreach (WorkflowEvent item in items)
        {
            await DeleteAsync(workflowEventId: item.Id);
        }
    }

    private void SecurityCheckEvent(WorkflowEvent workflowEvent)
    {
        int? appId = service.GetAppIdForWorkflowEvent(workflowEvent: workflowEvent);
        authorizationBroker.Authorize(userId: workflowEvent.ExecuteAs, appId: appId, privilege: "app_admin");
    }
}