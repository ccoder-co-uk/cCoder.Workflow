using System.Security;
using cCoder.Workflow.Brokers;
using cCoder.Data.Models.Workflow;


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
            flowInstanceDataBroker.GetAppId(flowInstanceData),
            $"{nameof(FlowInstanceData)}_create"
        );
        FlowInstanceData newFlowInstanceData = CreateStorageFlowInstanceData(flowInstanceData);

        FlowInstanceData result = await flowInstanceDataBroker.AddFlowInstanceDataAsync(
            newFlowInstanceData
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

    public async ValueTask<FlowInstanceData> AddQueuedAsync(FlowInstanceData flowInstanceData)
    {
        FlowInstanceData queuedFlowInstanceData = CreateQueuedStorageFlowInstanceData(flowInstanceData);

        FlowInstanceData result = await flowInstanceDataBroker.AddFlowInstanceDataAsync(
            queuedFlowInstanceData
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

    public async ValueTask<FlowInstanceData> UpdateAsync(FlowInstanceData flowInstanceData)
    {
        authorizationBroker.Authorize(
            flowInstanceDataBroker.GetAppId(flowInstanceData),
            $"{nameof(FlowInstanceData)}_update"
        );
        FlowInstanceData updateFlowInstanceData = CreateStorageFlowInstanceData(flowInstanceData);

        FlowInstanceData result = await flowInstanceDataBroker.UpdateFlowInstanceDataAsync(
            updateFlowInstanceData
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

    public async ValueTask DeleteAsync(Guid id)
    {
        FlowInstanceData flowInstanceData = Get(id);
        authorizationBroker.Authorize(
            flowInstanceDataBroker.GetAppId(flowInstanceData),
            $"{nameof(FlowInstanceData)}_delete"
        );
        _ = await flowInstanceDataBroker.DeleteFlowInstanceDataAsync(
            CreateStorageFlowInstanceData(flowInstanceData)
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












