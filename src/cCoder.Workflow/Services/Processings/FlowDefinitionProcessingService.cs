// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Dependencies;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations;

namespace cCoder.Workflow.Services.Processings;

internal sealed partial class FlowDefinitionProcessingService(
    IFlowDefinitionService service)
    : IFlowDefinitionProcessingService
{
    public bool AuthorizeFlowDefinitionExecution(string userId, int? appId) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [userId, appId]);
            return service.AuthorizeExecution(userId: userId, appId: appId);
        });

    public object ParseFlowDefinition(string definitionJson) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [definitionJson]);
            return service.ParseDefinition(definitionJson: definitionJson);
        });

    public object ParseFlowDefinitionData(string args) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [args]);
            return service.ParseData(args: args);
        });

    public string SerializeFlowDefinitionContext(object context) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [context]);
            return service.SerializeContext(context: context);
        });

    public FlowDefinition Get(Guid flowDefinitionId) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [flowDefinitionId]); return ExecuteGet(flowDefinitionId: flowDefinitionId); });

    private FlowDefinition ExecuteGet(Guid flowDefinitionId)
    {
        return service.Get(flowDefinitionId: flowDefinitionId);
    }

    public IQueryable<FlowDefinition> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<FlowDefinition> ExecuteGetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask<FlowDefinition> AddFlowDefinitionAsync(FlowDefinition newFlowDefinition) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [newFlowDefinition]); return await ExecuteAddAsync(entity: newFlowDefinition); }, isValueTask: true);

    private ValueTask<FlowDefinition> ExecuteAddAsync(FlowDefinition entity)
    {
        return service.AddFlowDefinitionAsync(newFlowDefinition: entity);
    }

    public ValueTask<FlowDefinition> UpdateFlowDefinitionAsync(FlowDefinition updatedFlowDefinition) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [updatedFlowDefinition]); return await ExecuteUpdateAsync(entity: updatedFlowDefinition); }, isValueTask: true);

    private ValueTask<FlowDefinition> ExecuteUpdateAsync(FlowDefinition entity)
    {
        return service.UpdateFlowDefinitionAsync(updatedFlowDefinition: entity);
    }

    public ValueTask DeleteAsync(Guid flowDefinitionId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [flowDefinitionId]); await ExecuteDeleteAsync(flowDefinitionId: flowDefinitionId); }, isValueTask: true);

    private ValueTask ExecuteDeleteAsync(Guid flowDefinitionId)
    {
        return service.DeleteWithInstancesAsync(flowDefinitionId: flowDefinitionId);
    }

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [appId]); await ExecuteDeleteByAppIdAsync(appId: appId); }, isValueTask: true);

    private ValueTask ExecuteDeleteByAppIdAsync(int appId) =>
        service.DeleteWithInstancesByAppIdAsync(appId: appId);

    public ValueTask<IEnumerable<Result<FlowDefinition>>> AddOrUpdateFlowDefinition(IEnumerable<FlowDefinition> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); return await ExecuteAddOrUpdate(items: items); }, isValueTask: true);

    private async ValueTask<IEnumerable<Result<FlowDefinition>>> ExecuteAddOrUpdate(IEnumerable<FlowDefinition> items)
    {
        FlowDefinition[] itemArray = items.ToArray();

        _ = service.LogFlowDefinitionAddOrUpdate(flowDefinitions: itemArray);

        List<Result<FlowDefinition>> results = new List<Result<FlowDefinition>>();

        foreach (FlowDefinition item in itemArray)
        {
            try
            {
                bool exists = item.Id != Guid.Empty
                    && GetAll(ignoreFilters: true)
                        .Any(predicate: flow => flow.Id == item.Id);

                FlowDefinition savedItem =
                    !exists
                        ? await AddFlowDefinitionAsync(newFlowDefinition: item)
                        : await UpdateFlowDefinitionAsync(updatedFlowDefinition: item);

                results.Add(item: new Result<FlowDefinition>
                {
                    Success = true,
                    Item = savedItem,
                    Message = !exists ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<FlowDefinition>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public ValueTask DeleteAllFlowDefinitionAsync(IEnumerable<FlowDefinition> deletedItems) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [deletedItems]); await ExecuteDeleteAllAsync(items: deletedItems); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAllAsync(IEnumerable<FlowDefinition> items)
    {
        foreach (FlowDefinition item in items)
        {
            await DeleteAsync(flowDefinitionId: item.Id);
        }
    }
}