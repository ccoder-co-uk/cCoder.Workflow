// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers;
using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations;

namespace cCoder.Workflow.Services.Processings;

internal sealed partial class FlowDefinitionProcessingService(IFlowDefinitionService service, IJsonBroker jsonBroker, ILogger<FlowDefinitionProcessingService> log) : IFlowDefinitionProcessingService
{
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

    public ValueTask<FlowDefinition> AddAsync(FlowDefinition entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); return await ExecuteAddAsync(entity: entity); }, isValueTask: true);

    private ValueTask<FlowDefinition> ExecuteAddAsync(FlowDefinition entity)
    {
        return service.AddAsync(flowDefinition: entity);
    }

    public ValueTask<FlowDefinition> UpdateAsync(FlowDefinition entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); return await ExecuteUpdateAsync(entity: entity); }, isValueTask: true);

    private ValueTask<FlowDefinition> ExecuteUpdateAsync(FlowDefinition entity)
    {
        return service.UpdateAsync(flowDefinition: entity);
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

    public ValueTask<IEnumerable<Result<FlowDefinition>>> AddOrUpdate(IEnumerable<FlowDefinition> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); return await ExecuteAddOrUpdate(items: items); }, isValueTask: true);

    private async ValueTask<IEnumerable<Result<FlowDefinition>>> ExecuteAddOrUpdate(IEnumerable<FlowDefinition> items)
    {
        FlowDefinition[] itemArray = items.ToArray();

        log.LogDebug(
            message: "AddOrUpdate:\n"
                + jsonBroker.Serialize(
                    value: itemArray.Select(
                        selector: item => new { item.Id, item.Name })));

        List<Result<FlowDefinition>> results = new List<Result<FlowDefinition>>();

        foreach (FlowDefinition item in itemArray)
        {
            try
            {
                FlowDefinition savedItem =
                    item.Id == Guid.Empty
                        ? await AddAsync(entity: item)
                        : await UpdateAsync(entity: item);

                results.Add(item: new Result<FlowDefinition>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
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

    public ValueTask DeleteAllAsync(IEnumerable<FlowDefinition> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); await ExecuteDeleteAllAsync(items: items); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAllAsync(IEnumerable<FlowDefinition> items)
    {
        foreach (FlowDefinition item in items)
        {
            await DeleteAsync(flowDefinitionId: item.Id);
        }
    }
}