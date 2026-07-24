// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Workflow.Brokers;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class FlowDefinitionService(
    IFlowDefinitionBroker flowDefinitionBroker,
    IAuthorizationBroker authorizationBroker
) : IFlowDefinitionService
{
    public FlowDefinition Get(Guid flowDefinitionId) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [flowDefinitionId]); return ExecuteGet(flowDefinitionId: flowDefinitionId); });

    private FlowDefinition ExecuteGet(Guid flowDefinitionId)
    {
        FlowDefinition flowDefinition = GetAll()
            .FirstOrDefault(predicate: i => i.Id == flowDefinitionId);

        if (flowDefinition is not null)
        {
            return flowDefinition;
        }

        FlowDefinition unrestrictedFlowDefinition = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: i => i.Id == flowDefinitionId);

        if (unrestrictedFlowDefinition is not null)
        {
            throw new SecurityException("Access Denied!");
        }

        return null;
    }

    public IQueryable<FlowDefinition> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<FlowDefinition> ExecuteGetAll(bool ignoreFilters = false) =>
        flowDefinitionBroker.GetAllFlowDefinitions(ignoreFilters: ignoreFilters);

    public ValueTask<FlowDefinition> AddAsync(FlowDefinition flowDefinition) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [flowDefinition]); return await ExecuteAddAsync(flowDefinition: flowDefinition); }, isValueTask: true);

    private async ValueTask<FlowDefinition> ExecuteAddAsync(FlowDefinition flowDefinition)
    {
        authorizationBroker.Authorize(appId: flowDefinition.AppId, privilege: $"{nameof(FlowDefinition)}_create");
        FlowDefinition newFlowDefinition = CreateStorageFlowDefinition(item: flowDefinition);
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        newFlowDefinition.CreatedOn = now;
        newFlowDefinition.CreatedBy = currentUserId;
        newFlowDefinition.LastUpdated = now;
        newFlowDefinition.LastUpdatedBy = currentUserId;

        FlowDefinition result = await flowDefinitionBroker.AddFlowDefinitionAsync(entity: newFlowDefinition);
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

    public ValueTask<FlowDefinition> UpdateAsync(FlowDefinition flowDefinition) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [flowDefinition]); return await ExecuteUpdateAsync(flowDefinition: flowDefinition); }, isValueTask: true);

    private async ValueTask<FlowDefinition> ExecuteUpdateAsync(FlowDefinition flowDefinition)
    {
        authorizationBroker.Authorize(appId: flowDefinition.AppId, privilege: $"{nameof(FlowDefinition)}_update");
        FlowDefinition updateFlowDefinition = CreateStorageFlowDefinition(item: flowDefinition);
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        updateFlowDefinition.LastUpdated = now;
        updateFlowDefinition.LastUpdatedBy = currentUserId;

        FlowDefinition result = await flowDefinitionBroker.UpdateFlowDefinitionAsync(
entity: updateFlowDefinition
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

    public ValueTask DeleteAsync(Guid flowDefinitionId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [flowDefinitionId]); await ExecuteDeleteAsync(flowDefinitionId: flowDefinitionId); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAsync(Guid flowDefinitionId)
    {
        FlowDefinition flowDefinition = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == flowDefinitionId);

        if (flowDefinition is null)
        {
            return;
        }

        authorizationBroker.Authorize(appId: flowDefinition.AppId, privilege: $"{nameof(FlowDefinition)}_delete");
        _ = await flowDefinitionBroker.DeleteFlowDefinitionAsync(entity: CreateStorageFlowDefinition(item: flowDefinition));
    }

    public ValueTask DeleteWithInstancesAsync(Guid flowDefinitionId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [flowDefinitionId]); await ExecuteDeleteWithInstancesAsync(flowDefinitionId: flowDefinitionId); }, isValueTask: true);

    private async ValueTask ExecuteDeleteWithInstancesAsync(Guid flowDefinitionId)
    {
        FlowDefinition flowDefinition = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == flowDefinitionId);

        if (flowDefinition is null)
        {
            return;
        }

        authorizationBroker.Authorize(appId: flowDefinition.AppId, privilege: $"{nameof(FlowDefinition)}_delete");
        await flowDefinitionBroker.DeleteFlowDefinitionWithInstancesAsync(flowDefinitionId: flowDefinitionId);
    }

    public ValueTask DeleteWithInstancesByAppIdAsync(int appId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [appId]); await ExecuteDeleteWithInstancesByAppIdAsync(appId: appId); }, isValueTask: true);

    private ValueTask ExecuteDeleteWithInstancesByAppIdAsync(int appId) =>
        flowDefinitionBroker.DeleteFlowDefinitionsWithInstancesByAppIdAsync(appId: appId);

    private static FlowDefinition CreateStorageFlowDefinition(FlowDefinition item) =>
        item == null
            ? null
            : new()
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
                App = item.App,
                Instances = item.Instances,
            };
}