using System.Security;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Workflow.Services.Processings;

internal class WorkflowEventProcessingService(IWorkflowEventService service, IFlowDefinitionService flowDefinitionService, IAuthorizationBroker authorizationBroker) : IWorkflowEventProcessingService
{
    public WorkflowEvent Get(Guid id)
    {
        return service.Get(id);
    }

    public IQueryable<WorkflowEvent> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

    public ValueTask<WorkflowEvent[]> GetSubscriptionsAsync(int appId, string eventContext)
    {
        WorkflowEvent[] subscriptions = service
            .GetAll(ignoreFilters: true)
            .Where(item => item.Flow.AppId == appId && item.EventContext == eventContext)
            .Include(item => item.Flow)
            .Include(item => item.ExecuteAsUser)
                .ThenInclude(user => user.Roles)
                    .ThenInclude(userRole => userRole.Role)
            .ToArray();

        return ValueTask.FromResult(subscriptions);
    }

    public ValueTask<WorkflowEvent> AddAsync(WorkflowEvent entity)
    {
        if (!SecurityCheckEvent(entity))
        {
            throw new SecurityException("Access Denied!");
        }
        return service.AddAsync(entity);
    }

    public ValueTask<WorkflowEvent> UpdateAsync(WorkflowEvent entity)
    {
        if (!SecurityCheckEvent(entity))
        {
            throw new SecurityException("Access Denied!");
        }
        return service.UpdateAsync(entity);
    }

    public ValueTask DeleteAsync(Guid id)
    {
        return service.DeleteAsync(id);
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
                        ? await AddAsync(item)
                        : await UpdateAsync(item);

                results.Add(new Result<WorkflowEvent>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result<WorkflowEvent>
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
            await DeleteAsync(item.Id);
        }
    }

    private bool SecurityCheckEvent(WorkflowEvent workflowEvent)
    {
        FlowDefinition flow = flowDefinitionService.GetAll().FirstOrDefault((FlowDefinition f) => f.Id == workflowEvent.FlowId);

        if (flow == null)
            throw new SecurityException("Access Denied!");

        authorizationBroker.Authorize(flow.AppId, "app_admin");

        return authorizationBroker.UserBelongsToApp(workflowEvent.ExecuteAs, flow.AppId);
    }
}
