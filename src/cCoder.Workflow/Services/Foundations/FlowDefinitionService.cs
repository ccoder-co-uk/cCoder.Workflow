using System.Security;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using DataFlowDefinition = cCoder.Data.Models.Workflow.FlowDefinition;
using IAuthorizationBroker = cCoder.Workflow.Brokers.IAuthorizationBroker;


namespace cCoder.Workflow.Services.Foundations;

internal class FlowDefinitionService(
    IFlowDefinitionBroker flowDefinitionBroker,
    IAuthorizationBroker authorizationBroker
) : IFlowDefinitionService
{
    public FlowDefinition Get(Guid id)
    {
        FlowDefinition flowDefinition = GetAll().FirstOrDefault(i => i.Id == id);
        if (flowDefinition is not null)
            return flowDefinition;

        FlowDefinition unrestrictedFlowDefinition = GetAll(true).FirstOrDefault(i => i.Id == id);
        if (unrestrictedFlowDefinition is not null)
            throw new SecurityException("Access Denied!");

        return null;
    }

    public IQueryable<FlowDefinition> GetAll(bool ignoreFilters = false) =>
        flowDefinitionBroker.GetAllFlowDefinitions(ignoreFilters);

    public async ValueTask<FlowDefinition> AddAsync(FlowDefinition flowDefinition)
    {
        authorizationBroker.Authorize(flowDefinition.AppId, $"{nameof(FlowDefinition)}_create");
        DataFlowDefinition newFlowDefinition = new()
        {
            Name = flowDefinition.Name,
            Description = flowDefinition.Description,
            LastUpdated = flowDefinition.LastUpdated,
            LastUpdatedBy = flowDefinition.LastUpdatedBy,
            CreatedOn = flowDefinition.CreatedOn,
            CreatedBy = flowDefinition.CreatedBy,
            AppId = flowDefinition.AppId,
            DefinitionJson = flowDefinition.DefinitionJson,
            ConfigJson = flowDefinition.ConfigJson,
            ReportingComponentName = flowDefinition.ReportingComponentName,
            InstanceReportingComponentName = flowDefinition.InstanceReportingComponentName,
        };
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        newFlowDefinition.CreatedOn = now;
        newFlowDefinition.CreatedBy = currentUserId;
        newFlowDefinition.LastUpdated = now;
        newFlowDefinition.LastUpdatedBy = currentUserId;

        DataFlowDefinition result = await flowDefinitionBroker.AddFlowDefinitionAsync(newFlowDefinition);
        flowDefinition.Id = result.Id;
        flowDefinition.Name = result.Name;
        flowDefinition.Description = result.Description;
        flowDefinition.LastUpdated = result.LastUpdated;
        flowDefinition.LastUpdatedBy = result.LastUpdatedBy;
        flowDefinition.CreatedOn = result.CreatedOn;
        flowDefinition.CreatedBy = result.CreatedBy;
        flowDefinition.AppId = result.AppId;
        flowDefinition.DefinitionJson = result.DefinitionJson;
        flowDefinition.ConfigJson = result.ConfigJson;
        flowDefinition.ReportingComponentName = result.ReportingComponentName;
        flowDefinition.InstanceReportingComponentName = result.InstanceReportingComponentName;
        return flowDefinition;
    }

    public async ValueTask<FlowDefinition> UpdateAsync(FlowDefinition flowDefinition)
    {
        authorizationBroker.Authorize(flowDefinition.AppId, $"{nameof(FlowDefinition)}_update");
        DataFlowDefinition updateFlowDefinition = new()
        {
            Id = flowDefinition.Id,
            Name = flowDefinition.Name,
            Description = flowDefinition.Description,
            LastUpdated = flowDefinition.LastUpdated,
            LastUpdatedBy = flowDefinition.LastUpdatedBy,
            CreatedOn = flowDefinition.CreatedOn,
            CreatedBy = flowDefinition.CreatedBy,
            AppId = flowDefinition.AppId,
            DefinitionJson = flowDefinition.DefinitionJson,
            ConfigJson = flowDefinition.ConfigJson,
            ReportingComponentName = flowDefinition.ReportingComponentName,
            InstanceReportingComponentName = flowDefinition.InstanceReportingComponentName,
        };
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        updateFlowDefinition.LastUpdated = now;
        updateFlowDefinition.LastUpdatedBy = currentUserId;

        DataFlowDefinition result = await flowDefinitionBroker.UpdateFlowDefinitionAsync(
            updateFlowDefinition
        );
        flowDefinition.Id = result.Id;
        flowDefinition.Name = result.Name;
        flowDefinition.Description = result.Description;
        flowDefinition.LastUpdated = result.LastUpdated;
        flowDefinition.LastUpdatedBy = result.LastUpdatedBy;
        flowDefinition.CreatedOn = result.CreatedOn;
        flowDefinition.CreatedBy = result.CreatedBy;
        flowDefinition.AppId = result.AppId;
        flowDefinition.DefinitionJson = result.DefinitionJson;
        flowDefinition.ConfigJson = result.ConfigJson;
        flowDefinition.ReportingComponentName = result.ReportingComponentName;
        flowDefinition.InstanceReportingComponentName = result.InstanceReportingComponentName;
        return flowDefinition;
    }

    public async ValueTask DeleteAsync(Guid id)
    {
        FlowDefinition flowDefinition = GetAll(ignoreFilters: true).FirstOrDefault(item => item.Id == id);

        if (flowDefinition is null)
            return;

        authorizationBroker.Authorize(flowDefinition.AppId, $"{nameof(FlowDefinition)}_delete");
        _ = await flowDefinitionBroker.DeleteFlowDefinitionAsync(ToExternalFlowDefinition(flowDefinition));
    }

    public async ValueTask DeleteWithInstancesAsync(Guid id)
    {
        FlowDefinition flowDefinition = GetAll(ignoreFilters: true).FirstOrDefault(item => item.Id == id);

        if (flowDefinition is null)
            return;

        authorizationBroker.Authorize(flowDefinition.AppId, $"{nameof(FlowDefinition)}_delete");
        await flowDefinitionBroker.DeleteFlowDefinitionWithInstancesAsync(id);
    }

    private static FlowDefinition ToExternalFlowDefinition(
        DataFlowDefinition item,
        App originalApp = null,
        ICollection<FlowInstanceData> originalInstances = null
    ) =>
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
            App = originalApp ?? (item.App == null ? null : ToLocalApp(item.App)),
            Instances = originalInstances ?? item.Instances?.Select(ToLocalFlowInstanceDataShallow).ToArray(),
        };

    static FlowDefinition ToLocalFlowDefinition(DataFlowDefinition item) =>
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
            App = item.App == null ? null : ToLocalApp(item.App),
            Instances = item.Instances?.Select(ToLocalFlowInstanceDataShallow).ToArray(),
        };

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
            App = item.App == null ? null : ToExternalApp(item.App),
            Instances = item.Instances?.Select(ToExternalFlowInstanceData).ToArray(),
        };

    static FlowInstanceData ToLocalFlowInstanceDataShallow(cCoder.Data.Models.Workflow.FlowInstanceData item) =>
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
        };

    static cCoder.Data.Models.Workflow.FlowInstanceData ToExternalFlowInstanceData(FlowInstanceData item) =>
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
        };

    static App ToLocalApp(cCoder.Data.Models.CMS.App item) =>
        new()
        {
            Id = item.Id,
            Name = item.Name,
            Domain = item.Domain,
        };

    static cCoder.Data.Models.CMS.App ToExternalApp(App item) =>
        new()
        {
            Id = item.Id,
            Name = item.Name,
            Domain = item.Domain,
        };
}












