// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Services.Orchestrations;

public interface IFlowInstanceDataOrchestrationService
{
    FlowInstanceData Get(Guid flowInstanceDataId);

    IQueryable<FlowInstanceData> GetAll(bool ignoreFilters = false);

    ValueTask<FlowInstanceData> AddFlowInstanceDataAsync(FlowInstanceData newEntity);

    ValueTask<FlowInstanceData> AddQueuedFlowInstanceDataAsync(FlowInstanceData newEntity);

    ValueTask<FlowInstanceData> UpdateFlowInstanceDataAsync(FlowInstanceData updatedEntity);

    ValueTask DeleteAsync(Guid flowInstanceDataId);

    ValueTask<IEnumerable<Result<FlowInstanceData>>> AddOrUpdateFlowInstanceData(IEnumerable<FlowInstanceData> items);

    ValueTask DeleteAllFlowInstanceDataAsync(IEnumerable<FlowInstanceData> deletedItems);
}