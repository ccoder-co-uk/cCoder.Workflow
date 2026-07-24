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

    ValueTask<FlowInstanceData> AddAsync(FlowInstanceData entity);

    ValueTask<FlowInstanceData> AddQueuedAsync(FlowInstanceData entity);

    ValueTask<FlowInstanceData> UpdateAsync(FlowInstanceData entity);

    ValueTask DeleteAsync(Guid flowInstanceDataId);

    ValueTask<IEnumerable<Result<FlowInstanceData>>> AddOrUpdate(IEnumerable<FlowInstanceData> items);

    ValueTask DeleteAllAsync(IEnumerable<FlowInstanceData> items);
}