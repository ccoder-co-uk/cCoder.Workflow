// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Foundations;

namespace cCoder.Workflow.Services.Processings;

internal sealed partial class FlowInstanceDataProcessingService(IFlowInstanceDataService service)
    : IFlowInstanceDataProcessingService
{
    public FlowInstanceData Get(Guid flowInstanceDataId) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [flowInstanceDataId]); return ExecuteGet(flowInstanceDataId: flowInstanceDataId); });

    private FlowInstanceData ExecuteGet(Guid flowInstanceDataId)
    {
        return service.Get(flowInstanceDataId: flowInstanceDataId);
    }

    public IQueryable<FlowInstanceData> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<FlowInstanceData> ExecuteGetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask<FlowInstanceData> AddAsync(FlowInstanceData entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); return await ExecuteAddAsync(entity: entity); }, isValueTask: true);

    private ValueTask<FlowInstanceData> ExecuteAddAsync(FlowInstanceData entity)
    {
        return service.AddAsync(flowInstanceData: entity);
    }

    public ValueTask<FlowInstanceData> AddQueuedAsync(FlowInstanceData entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); return await ExecuteAddQueuedAsync(entity: entity); }, isValueTask: true);

    private ValueTask<FlowInstanceData> ExecuteAddQueuedAsync(FlowInstanceData entity)
    {
        return service.AddQueuedAsync(flowInstanceData: entity);
    }

    public ValueTask<FlowInstanceData> UpdateAsync(FlowInstanceData entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); return await ExecuteUpdateAsync(entity: entity); }, isValueTask: true);

    private async ValueTask<FlowInstanceData> ExecuteUpdateAsync(FlowInstanceData entity)
    {
        FlowInstanceData dbVersion = service.Get(flowInstanceDataId: entity.Id);

        if (dbVersion == null)
        {
            throw new SecurityException("Access Denied!");
        }

        dbVersion.FlowDefinitionId = entity.FlowDefinitionId;
        dbVersion.Name = entity.Name;
        dbVersion.ContextString = entity.ContextString;
        dbVersion.State = entity.State;
        dbVersion.ReportingComponentName = entity.ReportingComponentName;
        dbVersion.Caller = entity.Caller;
        dbVersion.Start = entity.Start;
        dbVersion.End = entity.End;
        return await service.UpdateAsync(flowInstanceData: dbVersion);
    }

    public ValueTask DeleteAsync(Guid flowInstanceDataId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [flowInstanceDataId]); await ExecuteDeleteAsync(flowInstanceDataId: flowInstanceDataId); }, isValueTask: true);

    private ValueTask ExecuteDeleteAsync(Guid flowInstanceDataId)
    {
        return service.DeleteAsync(flowInstanceDataId: flowInstanceDataId);
    }

    public ValueTask<IEnumerable<Result<FlowInstanceData>>> AddOrUpdate(IEnumerable<FlowInstanceData> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); return await ExecuteAddOrUpdate(items: items); }, isValueTask: true);

    private async ValueTask<IEnumerable<Result<FlowInstanceData>>> ExecuteAddOrUpdate(IEnumerable<FlowInstanceData> items)
    {
        List<Result<FlowInstanceData>> results = new List<Result<FlowInstanceData>>();

        foreach (FlowInstanceData item in items)
        {
            try
            {
                FlowInstanceData savedItem =
                    item.Id == Guid.Empty
                        ? await AddAsync(entity: item)
                        : await UpdateAsync(entity: item);

                results.Add(item: new Result<FlowInstanceData>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<FlowInstanceData>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public ValueTask DeleteAllAsync(IEnumerable<FlowInstanceData> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); await ExecuteDeleteAllAsync(items: items); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAllAsync(IEnumerable<FlowInstanceData> items)
    {
        foreach (FlowInstanceData item in items)
        {
            await DeleteAsync(flowInstanceDataId: item.Id);
        }
    }
}