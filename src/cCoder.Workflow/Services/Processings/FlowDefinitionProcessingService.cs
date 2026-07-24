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

internal class FlowDefinitionProcessingService(IFlowDefinitionService service, IJsonBroker jsonBroker, ILogger<FlowDefinitionProcessingService> log) : IFlowDefinitionProcessingService
{
    public FlowDefinition Get(Guid flowDefinitionId)
    {
        return service.Get(flowDefinitionId: flowDefinitionId);
    }

    public IQueryable<FlowDefinition> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask<FlowDefinition> AddAsync(FlowDefinition entity)
    {
        return service.AddAsync(flowDefinition: entity);
    }

    public ValueTask<FlowDefinition> UpdateAsync(FlowDefinition entity)
    {
        return service.UpdateAsync(flowDefinition: entity);
    }

    public ValueTask DeleteAsync(Guid flowDefinitionId)
    {
        return service.DeleteWithInstancesAsync(flowDefinitionId: flowDefinitionId);
    }

    public ValueTask DeleteByAppIdAsync(int appId) =>
        service.DeleteWithInstancesByAppIdAsync(appId: appId);

    public async ValueTask<IEnumerable<Result<FlowDefinition>>> AddOrUpdate(IEnumerable<FlowDefinition> items)
    {
        FlowDefinition[] itemArray = items.ToArray();
        log.LogDebug(message: "AddOrUpdate:\n" + jsonBroker.Serialize(value: itemArray.Select(i => new { i.Id, i.Name })));
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

    public async ValueTask DeleteAllAsync(IEnumerable<FlowDefinition> items)
    {
        foreach (FlowDefinition item in items)
        {
            await DeleteAsync(flowDefinitionId: item.Id);
        }
    }
}