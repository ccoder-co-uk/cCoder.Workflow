// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Brokers;

public interface IFlowInstanceDataBroker
{
    IQueryable<FlowInstanceData> SelectAllFlowInstanceData();

    IQueryable<FlowInstanceData> SelectAllFlowInstanceDataIgnoringQueryFilters();

    ValueTask<FlowInstanceData> AddFlowInstanceDataAsync(FlowInstanceData newEntity);

    ValueTask<FlowInstanceData> UpdateFlowInstanceDataAsync(FlowInstanceData updatedEntity);

    ValueTask<int> DeleteFlowInstanceDataAsync(FlowInstanceData deletedEntity);

    int? SelectAppId(FlowInstanceData entity);
}