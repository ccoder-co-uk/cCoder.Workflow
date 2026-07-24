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

    public ValueTask<FlowInstanceData> AddFlowInstanceDataAsync(FlowInstanceData newEntity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [newEntity]); return await ExecuteAddAsync(entity: newEntity); }, isValueTask: true);

    private ValueTask<FlowInstanceData> ExecuteAddAsync(FlowInstanceData entity)
    {
        return service.AddFlowInstanceDataAsync(newFlowInstanceData: entity);
    }

    public ValueTask<FlowInstanceData> AddQueuedFlowInstanceDataAsync(FlowInstanceData newEntity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [newEntity]); return await ExecuteAddQueuedAsync(entity: newEntity); }, isValueTask: true);

    private ValueTask<FlowInstanceData> ExecuteAddQueuedAsync(FlowInstanceData entity)
    {
        return service.AddQueuedFlowInstanceDataAsync(newFlowInstanceData: entity);
    }

    public ValueTask<FlowInstanceData> UpdateFlowInstanceDataAsync(FlowInstanceData updatedEntity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [updatedEntity]); return await ExecuteUpdateAsync(entity: updatedEntity); }, isValueTask: true);

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
        return await service.UpdateFlowInstanceDataAsync(updatedFlowInstanceData: dbVersion);
    }

    public ValueTask DeleteAsync(Guid flowInstanceDataId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [flowInstanceDataId]); await ExecuteDeleteAsync(flowInstanceDataId: flowInstanceDataId); }, isValueTask: true);

    private ValueTask ExecuteDeleteAsync(Guid flowInstanceDataId)
    {
        return service.DeleteAsync(flowInstanceDataId: flowInstanceDataId);
    }

    public ValueTask<IEnumerable<Result<FlowInstanceData>>> AddOrUpdateFlowInstanceData(IEnumerable<FlowInstanceData> items) =>
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
                        ? await AddFlowInstanceDataAsync(newEntity: item)
                        : await UpdateFlowInstanceDataAsync(updatedEntity: item);

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

    public ValueTask DeleteAllFlowInstanceDataAsync(IEnumerable<FlowInstanceData> deletedItems) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [deletedItems]); await ExecuteDeleteAllAsync(items: deletedItems); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAllAsync(IEnumerable<FlowInstanceData> items)
    {
        foreach (FlowInstanceData item in items)
        {
            await DeleteAsync(flowInstanceDataId: item.Id);
        }
    }
}