using cCoder.Data;
using cCoder.Workflow.Brokers.Events;
using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Eventing.Models;
using DataFlowDefinition = cCoder.Data.Models.Workflow.FlowDefinition;
using DataUser = cCoder.Data.Models.Security.User;
using DataWorkflowEvent = cCoder.Data.Models.Workflow.WorkflowEvent;


namespace cCoder.Workflow.Services.Foundations.Events;

internal class WorkflowEventEventService(
    IWorkflowEventEventBroker workflowEventEventBroker,
    ICoreAuthInfo authInfo
) : IWorkflowEventEventService
{
    public async ValueTask RaiseWorkflowEventAddEventAsync(WorkflowEvent entity)
    {
        EventMessage<DataWorkflowEvent> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalWorkflowEvent(entity),
        };

        await workflowEventEventBroker.RaiseWorkflowEventAddEventAsync(message);
    }

    public async ValueTask RaiseWorkflowEventUpdateEventAsync(WorkflowEvent entity)
    {
        EventMessage<DataWorkflowEvent> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalWorkflowEvent(entity),
        };

        await workflowEventEventBroker.RaiseWorkflowEventUpdateEventAsync(message);
    }

    public async ValueTask RaiseWorkflowEventDeleteEventAsync(WorkflowEvent entity)
    {
        EventMessage<DataWorkflowEvent> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalWorkflowEvent(entity),
        };

        await workflowEventEventBroker.RaiseWorkflowEventDeleteEventAsync(message);
    }

    static DataWorkflowEvent ToExternalWorkflowEvent(WorkflowEvent item) =>
        new()
        {
            Id = item.Id,
            Type = item.Type,
            EventContext = item.EventContext,
            CreatedBy = item.CreatedBy,
            CreatedOn = item.CreatedOn,
            FlowId = item.FlowId,
            Flow = item.Flow == null ? null : new DataFlowDefinition
            {
                Id = item.Flow.Id,
                Name = item.Flow.Name,
                Description = item.Flow.Description,
                LastUpdated = item.Flow.LastUpdated,
                LastUpdatedBy = item.Flow.LastUpdatedBy,
                CreatedOn = item.Flow.CreatedOn,
                CreatedBy = item.Flow.CreatedBy,
                AppId = item.Flow.AppId,
                DefinitionJson = item.Flow.DefinitionJson,
                ConfigJson = item.Flow.ConfigJson,
                ReportingComponentName = item.Flow.ReportingComponentName,
                InstanceReportingComponentName = item.Flow.InstanceReportingComponentName,
            },
            ExecuteAs = item.ExecuteAs,
            ExecuteAsUser = item.ExecuteAsUser == null ? null : new DataUser
            {
                Id = item.ExecuteAsUser.Id,
                DisplayName = item.ExecuteAsUser.DisplayName,
                Email = item.ExecuteAsUser.Email,
            },
        };
}









