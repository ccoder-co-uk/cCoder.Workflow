using System.Security;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using DataFlowInstanceData = cCoder.Data.Models.Workflow.FlowInstanceData;
using IAuthorizationBroker = cCoder.Workflow.Brokers.IAuthorizationBroker;


namespace cCoder.Workflow.Services.Foundations;

internal class FlowInstanceDataService(
    IFlowInstanceDataBroker flowInstanceDataBroker,
    IAuthorizationBroker authorizationBroker
) : IFlowInstanceDataService
{
    public FlowInstanceData Get(Guid id)
    {
        FlowInstanceData flowInstanceData = GetAll().FirstOrDefault(i => i.Id == id);
        if (flowInstanceData is not null)
            return flowInstanceData;

        FlowInstanceData unrestrictedFlowInstanceData = GetAll(true).FirstOrDefault(i => i.Id == id);
        if (unrestrictedFlowInstanceData is not null)
            throw new SecurityException("Access Denied!");

        return null;
    }

    public IQueryable<FlowInstanceData> GetAll(bool ignoreFilters = false) =>
        flowInstanceDataBroker.GetAllFlowInstanceData(ignoreFilters);

    public async ValueTask<FlowInstanceData> AddAsync(FlowInstanceData flowInstanceData)
    {
        authorizationBroker.Authorize(
            flowInstanceDataBroker.GetAppId(ToExternalFlowInstanceData(flowInstanceData)),
            $"{nameof(FlowInstanceData)}_create"
        );
        DataFlowInstanceData newFlowInstanceData = new()
        {
            FlowDefinitionId = flowInstanceData.FlowDefinitionId,
            Name = flowInstanceData.Name,
            ContextJson = flowInstanceData.ContextJson,
            State = flowInstanceData.State,
            ReportingComponentName = flowInstanceData.ReportingComponentName,
            Caller = flowInstanceData.Caller,
            Start = flowInstanceData.Start,
            End = flowInstanceData.End,
        };

        newFlowInstanceData = await flowInstanceDataBroker.AddFlowInstanceDataAsync(
            newFlowInstanceData
        );
        return ToExternalFlowInstanceData(newFlowInstanceData, flowInstanceData.FlowDefinition);
    }

    public async ValueTask<FlowInstanceData> UpdateAsync(FlowInstanceData flowInstanceData)
    {
        authorizationBroker.Authorize(
            flowInstanceDataBroker.GetAppId(ToExternalFlowInstanceData(flowInstanceData)),
            $"{nameof(FlowInstanceData)}_update"
        );
        DataFlowInstanceData updateFlowInstanceData = new()
        {
            Id = flowInstanceData.Id,
            FlowDefinitionId = flowInstanceData.FlowDefinitionId,
            Name = flowInstanceData.Name,
            ContextJson = flowInstanceData.ContextJson,
            State = flowInstanceData.State,
            ReportingComponentName = flowInstanceData.ReportingComponentName,
            Caller = flowInstanceData.Caller,
            Start = flowInstanceData.Start,
            End = flowInstanceData.End,
        };

        updateFlowInstanceData = await flowInstanceDataBroker.UpdateFlowInstanceDataAsync(
            updateFlowInstanceData
        );
        return ToExternalFlowInstanceData(updateFlowInstanceData, flowInstanceData.FlowDefinition);
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        FlowInstanceData flowInstanceData = Get(id);
        authorizationBroker.Authorize(
            flowInstanceDataBroker.GetAppId(ToExternalFlowInstanceData(flowInstanceData)),
            $"{nameof(FlowInstanceData)}_delete"
        );
        _ = await flowInstanceDataBroker.DeleteFlowInstanceDataAsync(ToExternalFlowInstanceData(flowInstanceData));
    }

    private static FlowInstanceData ToExternalFlowInstanceData(
        DataFlowInstanceData item,
        FlowDefinition originalFlowDefinition = null
    ) =>
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
            End = item.End ?? default,
            FlowDefinition = originalFlowDefinition ?? (item.FlowDefinition == null ? null : ToLocalFlowDefinitionShallow(item.FlowDefinition)),
        };

    static FlowInstanceData ToLocalFlowInstanceData(DataFlowInstanceData item) =>
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
            End = item.End ?? default,
            FlowDefinition = item.FlowDefinition == null ? null : ToLocalFlowDefinitionShallow(item.FlowDefinition),
        };

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
            FlowDefinition = item.FlowDefinition == null ? null : new cCoder.Data.Models.Workflow.FlowDefinition
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

    static FlowDefinition ToLocalFlowDefinitionShallow(cCoder.Data.Models.Workflow.FlowDefinition item) =>
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
        };
}












