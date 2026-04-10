using cCoder.Workflow.Brokers;
using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations;

namespace cCoder.Workflow.Services.Processings;

internal class FlowDefinitionProcessingService(IFlowDefinitionService service, IJsonBroker jsonBroker, ILogger<FlowDefinitionProcessingService> log) : IFlowDefinitionProcessingService
{
    public FlowDefinition Get(Guid id)
    {
        return service.Get(id);
    }

    public IQueryable<FlowDefinition> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

    public ValueTask<FlowDefinition> AddAsync(FlowDefinition entity)
    {
        return service.AddAsync(entity);
    }

    public ValueTask<FlowDefinition> UpdateAsync(FlowDefinition entity)
    {
        return service.UpdateAsync(entity);
    }

    public ValueTask DeleteAsync(Guid id)
    {
        return service.DeleteWithInstancesAsync(id);
    }

    public async ValueTask<IEnumerable<Result<FlowDefinition>>> AddOrUpdate(IEnumerable<FlowDefinition> items)
    {
        FlowDefinition[] itemArray = items.ToArray();
        log.LogDebug("AddOrUpdate:\n" + jsonBroker.Serialize(itemArray.Select(i => new { i.Id, i.Name })));
        List<Result<FlowDefinition>> results = new List<Result<FlowDefinition>>();

        foreach (FlowDefinition item in itemArray)
        {
            try
            {
                FlowDefinition savedItem =
                    item.Id == Guid.Empty
                        ? await AddAsync(item)
                        : await UpdateAsync(item);

                results.Add(new Result<FlowDefinition>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result<FlowDefinition>
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
            await DeleteAsync(item.Id);
        }
    }
}
