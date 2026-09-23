// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Foundations;

internal interface IFlowInstanceDataService
{
    object CreateSingleResult<T>(IQueryable<T> queryable);

    FlowInstanceData Get(Guid flowInstanceDataId);

    IQueryable<FlowInstanceData> GetAll(bool ignoreFilters = false);

    ValueTask<FlowInstanceData> AddFlowInstanceDataAsync(FlowInstanceData newFlowInstanceData);

    ValueTask<FlowInstanceData> AddQueuedFlowInstanceDataAsync(FlowInstanceData newFlowInstanceData);

    ValueTask<FlowInstanceData> UpdateFlowInstanceDataAsync(FlowInstanceData updatedFlowInstanceData);

    ValueTask DeleteAsync(Guid flowInstanceDataId);
}