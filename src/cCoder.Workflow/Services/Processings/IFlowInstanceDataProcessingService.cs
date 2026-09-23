// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Services.Processings;

internal interface IFlowInstanceDataProcessingService
{
    object CreateSingleResult<T>(IQueryable<T> queryable);

    FlowInstanceData Get(Guid flowInstanceDataId);

    IQueryable<FlowInstanceData> GetAll(bool ignoreFilters = false);

    ValueTask<FlowInstanceData> AddFlowInstanceDataAsync(FlowInstanceData newFlowInstanceData);

    ValueTask<FlowInstanceData> AddQueuedFlowInstanceDataAsync(FlowInstanceData newFlowInstanceData);

    ValueTask<FlowInstanceData> UpdateFlowInstanceDataAsync(FlowInstanceData updatedFlowInstanceData);

    ValueTask DeleteAsync(Guid flowInstanceDataId);

    ValueTask<IEnumerable<Result<FlowInstanceData>>> AddOrUpdateFlowInstanceData(IEnumerable<FlowInstanceData> items);

    ValueTask DeleteAllFlowInstanceDataAsync(IEnumerable<FlowInstanceData> deletedItems);
}