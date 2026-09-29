// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;

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