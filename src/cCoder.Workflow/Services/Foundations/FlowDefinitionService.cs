// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Workflow.Brokers;
using cCoder.Data.Models.Workflow;


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
        FlowDefinition newFlowDefinition = CreateStorageFlowDefinition(flowDefinition);
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        newFlowDefinition.CreatedOn = now;
        newFlowDefinition.CreatedBy = currentUserId;
        newFlowDefinition.LastUpdated = now;
        newFlowDefinition.LastUpdatedBy = currentUserId;

        FlowDefinition result = await flowDefinitionBroker.AddFlowDefinitionAsync(newFlowDefinition);
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
        FlowDefinition updateFlowDefinition = CreateStorageFlowDefinition(flowDefinition);
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        updateFlowDefinition.LastUpdated = now;
        updateFlowDefinition.LastUpdatedBy = currentUserId;

        FlowDefinition result = await flowDefinitionBroker.UpdateFlowDefinitionAsync(
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
        _ = await flowDefinitionBroker.DeleteFlowDefinitionAsync(CreateStorageFlowDefinition(flowDefinition));
    }

    public async ValueTask DeleteWithInstancesAsync(Guid id)
    {
        FlowDefinition flowDefinition = GetAll(ignoreFilters: true).FirstOrDefault(item => item.Id == id);

        if (flowDefinition is null)
            return;

        authorizationBroker.Authorize(flowDefinition.AppId, $"{nameof(FlowDefinition)}_delete");
        await flowDefinitionBroker.DeleteFlowDefinitionWithInstancesAsync(id);
    }

    public ValueTask DeleteWithInstancesByAppIdAsync(int appId) =>
        flowDefinitionBroker.DeleteFlowDefinitionsWithInstancesByAppIdAsync(appId);

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