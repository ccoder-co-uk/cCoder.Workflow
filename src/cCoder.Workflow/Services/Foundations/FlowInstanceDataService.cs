// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Workflow.Brokers;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class FlowInstanceDataService(
    IFlowInstanceDataBroker flowInstanceDataBroker,
    IAuthorizationBroker authorizationBroker
) : IFlowInstanceDataService
{
    public FlowInstanceData Get(Guid flowInstanceDataId) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [flowInstanceDataId]); return ExecuteGet(flowInstanceDataId: flowInstanceDataId); });

    private FlowInstanceData ExecuteGet(Guid flowInstanceDataId)
    {
        FlowInstanceData flowInstanceData = GetAll()
            .FirstOrDefault(predicate: i => i.Id == flowInstanceDataId);

        if (flowInstanceData is not null)
        {
            return flowInstanceData;
        }

        FlowInstanceData unrestrictedFlowInstanceData = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: i => i.Id == flowInstanceDataId);

        if (unrestrictedFlowInstanceData is not null)
        {
            throw new SecurityException("Access Denied!");
        }

        return null;
    }

    public IQueryable<FlowInstanceData> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () => { ValidateAllOnGet(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<FlowInstanceData> ExecuteGetAll(bool ignoreFilters = false) =>
        flowInstanceDataBroker.GetAllFlowInstanceData(ignoreFilters: ignoreFilters);

    public ValueTask<FlowInstanceData> AddAsync(FlowInstanceData flowInstanceData) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [flowInstanceData]); return await ExecuteAddAsync(flowInstanceData: flowInstanceData); }, isValueTask: true);

    private async ValueTask<FlowInstanceData> ExecuteAddAsync(FlowInstanceData flowInstanceData)
    {
        authorizationBroker.Authorize(
appId: flowInstanceDataBroker.SelectAppId(entity: flowInstanceData),
privilege: $"{nameof(FlowInstanceData)}_create"
        );

        FlowInstanceData newFlowInstanceData = CreateStorageFlowInstanceData(item: flowInstanceData);

        FlowInstanceData result = await flowInstanceDataBroker.AddFlowInstanceDataAsync(
entity: newFlowInstanceData
        );

        flowInstanceData.Id = result.Id;
        flowInstanceData.FlowDefinitionId = result.FlowDefinitionId;
        flowInstanceData.Name = result.Name;
        flowInstanceData.ContextJson = result.ContextJson;
        flowInstanceData.State = result.State;
        flowInstanceData.ReportingComponentName = result.ReportingComponentName;
        flowInstanceData.Caller = result.Caller;
        flowInstanceData.Start = result.Start;
        flowInstanceData.End = result.End ?? default;
        return flowInstanceData;
    }

    public ValueTask<FlowInstanceData> AddQueuedAsync(FlowInstanceData flowInstanceData) =>
        TryCatch(operation: async () => { ValidateQueuedOnAdd(inputs: [flowInstanceData]); return await ExecuteAddQueuedAsync(flowInstanceData: flowInstanceData); }, isValueTask: true);

    private async ValueTask<FlowInstanceData> ExecuteAddQueuedAsync(FlowInstanceData flowInstanceData)
    {
        FlowInstanceData queuedFlowInstanceData = CreateQueuedStorageFlowInstanceData(item: flowInstanceData);

        FlowInstanceData result = await flowInstanceDataBroker.AddFlowInstanceDataAsync(
entity: queuedFlowInstanceData
        );

        flowInstanceData.Id = result.Id;
        flowInstanceData.FlowDefinitionId = result.FlowDefinitionId;
        flowInstanceData.Name = result.Name;
        flowInstanceData.ContextJson = result.ContextJson;
        flowInstanceData.State = result.State;
        flowInstanceData.ReportingComponentName = result.ReportingComponentName;
        flowInstanceData.Caller = result.Caller;
        flowInstanceData.Start = result.Start;
        flowInstanceData.End = result.End ?? default;
        return flowInstanceData;
    }

    public ValueTask<FlowInstanceData> UpdateAsync(FlowInstanceData flowInstanceData) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [flowInstanceData]); return await ExecuteUpdateAsync(flowInstanceData: flowInstanceData); }, isValueTask: true);

    private async ValueTask<FlowInstanceData> ExecuteUpdateAsync(FlowInstanceData flowInstanceData)
    {
        authorizationBroker.Authorize(
appId: flowInstanceDataBroker.SelectAppId(entity: flowInstanceData),
privilege: $"{nameof(FlowInstanceData)}_update"
        );

        FlowInstanceData updateFlowInstanceData = CreateStorageFlowInstanceData(item: flowInstanceData);

        FlowInstanceData result = await flowInstanceDataBroker.UpdateFlowInstanceDataAsync(
entity: updateFlowInstanceData
        );

        flowInstanceData.Id = result.Id;
        flowInstanceData.FlowDefinitionId = result.FlowDefinitionId;
        flowInstanceData.Name = result.Name;
        flowInstanceData.ContextJson = result.ContextJson;
        flowInstanceData.State = result.State;
        flowInstanceData.ReportingComponentName = result.ReportingComponentName;
        flowInstanceData.Caller = result.Caller;
        flowInstanceData.Start = result.Start;
        flowInstanceData.End = result.End ?? default;
        return flowInstanceData;
    }

    public ValueTask DeleteAsync(Guid flowInstanceDataId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [flowInstanceDataId]); await ExecuteDeleteAsync(flowInstanceDataId: flowInstanceDataId); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAsync(Guid flowInstanceDataId)
    {
        FlowInstanceData flowInstanceData = Get(flowInstanceDataId: flowInstanceDataId);

        authorizationBroker.Authorize(
appId: flowInstanceDataBroker.SelectAppId(entity: flowInstanceData),
privilege: $"{nameof(FlowInstanceData)}_delete"
        );

        _ = await flowInstanceDataBroker.DeleteFlowInstanceDataAsync(
entity: CreateStorageFlowInstanceData(item: flowInstanceData)
        );
    }

    private static FlowInstanceData CreateStorageFlowInstanceData(FlowInstanceData item) =>
        item == null
            ? null
            : new()
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
                FlowDefinition = item.FlowDefinition,
            };

    private static FlowInstanceData CreateQueuedStorageFlowInstanceData(FlowInstanceData item) =>
        item == null
            ? null
            : new()
            {
                Id = item.Id,
                FlowDefinitionId = item.FlowDefinitionId,
                Name = item.Name,
                ContextString = item.ContextString,
                State = item.State,
                ReportingComponentName = item.ReportingComponentName,
                Caller = item.Caller,
                Start = item.Start,
                End = item.End,
            };
}