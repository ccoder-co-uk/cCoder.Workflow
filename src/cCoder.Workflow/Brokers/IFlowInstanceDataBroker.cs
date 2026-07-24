// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Brokers;

public interface IFlowInstanceDataBroker
{
    IQueryable<FlowInstanceData> SelectAllFlowInstanceData();

    IQueryable<FlowInstanceData> SelectAllFlowInstanceDataIgnoringQueryFilters();

    ValueTask<FlowInstanceData> AddFlowInstanceDataAsync(FlowInstanceData entity);

    ValueTask<FlowInstanceData> UpdateFlowInstanceDataAsync(FlowInstanceData entity);

    ValueTask<int> DeleteFlowInstanceDataAsync(FlowInstanceData entity);

    int? SelectAppId(FlowInstanceData entity);
}