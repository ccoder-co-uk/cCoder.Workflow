// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Services.Processings;

public interface IFlowInstanceDataProcessingService
{
    FlowInstanceData Get(Guid id);

    IQueryable<FlowInstanceData> GetAll(bool ignoreFilters = false);

    ValueTask<FlowInstanceData> AddAsync(FlowInstanceData entity);

    ValueTask<FlowInstanceData> AddQueuedAsync(FlowInstanceData entity);

    ValueTask<FlowInstanceData> UpdateAsync(FlowInstanceData entity);

    ValueTask DeleteAsync(Guid id);

    ValueTask<IEnumerable<Result<FlowInstanceData>>> AddOrUpdate(IEnumerable<FlowInstanceData> items);

    ValueTask DeleteAllAsync(IEnumerable<FlowInstanceData> items);
}