// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Brokers;

public interface IFlowInstanceDataBroker
{
    IQueryable<FlowInstanceData> SelectAllFlowInstanceData();

    IQueryable<FlowInstanceData> SelectAllFlowInstanceDataIgnoringQueryFilters();

    ValueTask<FlowInstanceData> AddFlowInstanceDataAsync(FlowInstanceData newFlowInstanceData);

    ValueTask<FlowInstanceData> UpdateFlowInstanceDataAsync(FlowInstanceData updatedFlowInstanceData);

    ValueTask<int> DeleteFlowInstanceDataAsync(FlowInstanceData deletedFlowInstanceData);

    int? SelectAppId(FlowInstanceData flowInstanceData);
}