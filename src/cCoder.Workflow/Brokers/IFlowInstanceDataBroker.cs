// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Brokers;

public interface IFlowInstanceDataBroker
{
    IQueryable<FlowInstanceData> GetAllFlowInstanceData(bool ignoreFilters);
    ValueTask<FlowInstanceData> AddFlowInstanceDataAsync(FlowInstanceData entity);
    ValueTask<FlowInstanceData> UpdateFlowInstanceDataAsync(FlowInstanceData entity);
    ValueTask<int> DeleteFlowInstanceDataAsync(FlowInstanceData entity);
    ValueTask DeleteAllFlowInstanceDataAsync(IEnumerable<FlowInstanceData> items);
    int? GetAppId(FlowInstanceData entity);
}