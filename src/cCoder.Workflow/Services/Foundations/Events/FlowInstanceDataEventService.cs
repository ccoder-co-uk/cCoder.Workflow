using cCoder.Data;
using cCoder.Workflow.Brokers.Events;
using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using EventLibrary.Models;
using DataFlowDefinition = cCoder.Data.Models.Workflow.FlowDefinition;
using DataFlowInstanceData = cCoder.Data.Models.Workflow.FlowInstanceData;


namespace cCoder.Workflow.Services.Foundations.Events;

internal class FlowInstanceDataEventService(
    IFlowInstanceDataEventBroker flowInstanceDataEventBroker,
    ICoreAuthInfo authInfo
) : IFlowInstanceDataEventService
{
    public async ValueTask RaiseFlowInstanceDataAddEventAsync(FlowInstanceData entity)
    {
        EventMessage<DataFlowInstanceData> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalFlowInstanceData(entity),
        };

        await flowInstanceDataEventBroker.RaiseFlowInstanceDataAddEventAsync(message);
    }

    public async ValueTask RaiseFlowInstanceDataUpdateEventAsync(FlowInstanceData entity)
    {
        EventMessage<DataFlowInstanceData> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalFlowInstanceData(entity),
        };

        await flowInstanceDataEventBroker.RaiseFlowInstanceDataUpdateEventAsync(message);
    }

    public async ValueTask RaiseFlowInstanceDataDeleteEventAsync(FlowInstanceData entity)
    {
        EventMessage<DataFlowInstanceData> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalFlowInstanceData(entity),
        };

        await flowInstanceDataEventBroker.RaiseFlowInstanceDataDeleteEventAsync(message);
    }

    static DataFlowInstanceData ToExternalFlowInstanceData(FlowInstanceData item) =>
        new()
        {
            Id = item.Id,
            FlowDefinitionId = item.FlowDefinitionId,
            Name = item.Name,
            ContextJson = item.ContextJson,
            State = item.State,
            ReportingComponentName = item.ReportingComponentName,
            Caller = item.Caller,
            Start = item.Start,
            End = item.End,
            FlowDefinition = item.FlowDefinition == null ? null : new DataFlowDefinition
            {
                Id = item.FlowDefinition.Id,
                Name = item.FlowDefinition.Name,
                Description = item.FlowDefinition.Description,
                LastUpdated = item.FlowDefinition.LastUpdated,
                LastUpdatedBy = item.FlowDefinition.LastUpdatedBy,
                CreatedOn = item.FlowDefinition.CreatedOn,
                CreatedBy = item.FlowDefinition.CreatedBy,
                AppId = item.FlowDefinition.AppId,
                DefinitionJson = item.FlowDefinition.DefinitionJson,
                ConfigJson = item.FlowDefinition.ConfigJson,
                ReportingComponentName = item.FlowDefinition.ReportingComponentName,
                InstanceReportingComponentName = item.FlowDefinition.InstanceReportingComponentName,
            },
        };
}









