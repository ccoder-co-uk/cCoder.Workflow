using cCoder.Data;
using cCoder.Workflow.Brokers.Events;
using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Eventing.Models;
using DataApp = cCoder.Data.Models.CMS.App;
using DataFlowDefinition = cCoder.Data.Models.Workflow.FlowDefinition;
using DataFlowInstanceData = cCoder.Data.Models.Workflow.FlowInstanceData;


namespace cCoder.Workflow.Services.Foundations.Events;

internal class FlowDefinitionEventService(
    IFlowDefinitionEventBroker flowDefinitionEventBroker,
    ICoreAuthInfo authInfo
) : IFlowDefinitionEventService
{
    public async ValueTask RaiseFlowDefinitionAddEventAsync(FlowDefinition entity)
    {
        EventMessage<DataFlowDefinition> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalFlowDefinition(entity),
        };

        await flowDefinitionEventBroker.RaiseFlowDefinitionAddEventAsync(message);
    }

    public async ValueTask RaiseFlowDefinitionUpdateEventAsync(FlowDefinition entity)
    {
        EventMessage<DataFlowDefinition> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalFlowDefinition(entity),
        };

        await flowDefinitionEventBroker.RaiseFlowDefinitionUpdateEventAsync(message);
    }

    public async ValueTask RaiseFlowDefinitionDeleteEventAsync(FlowDefinition entity)
    {
        EventMessage<DataFlowDefinition> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalFlowDefinition(entity),
        };

        await flowDefinitionEventBroker.RaiseFlowDefinitionDeleteEventAsync(message);
    }

    static DataFlowDefinition ToExternalFlowDefinition(FlowDefinition item) =>
        new()
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            LastUpdated = item.LastUpdated,
            LastUpdatedBy = item.LastUpdatedBy,
            CreatedOn = item.CreatedOn,
            CreatedBy = item.CreatedBy,
            AppId = item.AppId,
            DefinitionJson = item.DefinitionJson,
            ConfigJson = item.ConfigJson,
            ReportingComponentName = item.ReportingComponentName,
            InstanceReportingComponentName = item.InstanceReportingComponentName,
            App = item.App == null ? null : new DataApp
            {
                Id = item.App.Id,
                Name = item.App.Name,
                Domain = item.App.Domain,
            },
            Instances = item.Instances?.Select(instance => new DataFlowInstanceData
            {
                Id = instance.Id,
                FlowDefinitionId = instance.FlowDefinitionId,
                Name = instance.Name,
                ContextJson = instance.ContextJson,
                State = instance.State,
                ReportingComponentName = instance.ReportingComponentName,
                Caller = instance.Caller,
                Start = instance.Start,
                End = instance.End,
            }).ToArray(),
        };
}









