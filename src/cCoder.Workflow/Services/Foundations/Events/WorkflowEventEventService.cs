// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Workflow.Brokers.Events;
using cCoder.Data.Models.Workflow;
using cCoder.Eventing.Models;


namespace cCoder.Workflow.Services.Foundations.Events;

internal class WorkflowEventEventService(
    IWorkflowEventEventBroker workflowEventEventBroker,
    ICoreAuthInfo authInfo
) : IWorkflowEventEventService
{
    public async ValueTask RaiseWorkflowEventAddEventAsync(WorkflowEvent entity)
    {
        EventMessage<WorkflowEvent> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await workflowEventEventBroker.RaiseWorkflowEventAddEventAsync(message:message);
    }

    public async ValueTask RaiseWorkflowEventUpdateEventAsync(WorkflowEvent entity)
    {
        EventMessage<WorkflowEvent> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await workflowEventEventBroker.RaiseWorkflowEventUpdateEventAsync(message:message);
    }

    public async ValueTask RaiseWorkflowEventDeleteEventAsync(WorkflowEvent entity)
    {
        EventMessage<WorkflowEvent> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await workflowEventEventBroker.RaiseWorkflowEventDeleteEventAsync(message:message);
    }
}