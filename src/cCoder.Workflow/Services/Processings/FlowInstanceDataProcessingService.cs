using System.Security;
using cCoder.Workflow.Models;
using cCoder.Workflow.Brokers;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations;

namespace cCoder.Workflow.Services.Processings;

internal class FlowInstanceDataProcessingService(
    IFlowInstanceDataService service,
    IFlowInstanceDataBroker flowInstanceDataBroker)
    : IFlowInstanceDataProcessingService
{
    public FlowInstanceData Get(Guid id)
    {
        return service.Get(id);
    }

    public IQueryable<FlowInstanceData> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

    public ValueTask<FlowInstanceData> AddAsync(FlowInstanceData entity)
    {
        return service.AddAsync(entity);
    }

    public ValueTask<FlowInstanceData> AddQueuedAsync(FlowInstanceData entity)
    {
        FlowInstanceData queuedEntity = new()
        {
            Id = entity.Id,
            FlowDefinitionId = entity.FlowDefinitionId,
            Name = entity.Name,
            ContextString = entity.ContextString,
            State = entity.State,
            ReportingComponentName = entity.ReportingComponentName,
            Caller = entity.Caller,
            Start = entity.Start,
            End = entity.End,
        };

        return flowInstanceDataBroker.AddFlowInstanceDataAsync(queuedEntity);
    }

    public async ValueTask<FlowInstanceData> UpdateAsync(FlowInstanceData entity)
    {
        FlowInstanceData dbVersion = service.Get(entity.Id);
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
        return await service.UpdateAsync(dbVersion);
    }

    public ValueTask DeleteAsync(Guid id)
    {
        return service.DeleteAsync(id);
    }

    public async ValueTask<IEnumerable<Result<FlowInstanceData>>> AddOrUpdate(IEnumerable<FlowInstanceData> items)
    {
        List<Result<FlowInstanceData>> results = new List<Result<FlowInstanceData>>();

        foreach (FlowInstanceData item in items)
        {
            try
            {
                FlowInstanceData savedItem =
                    item.Id == Guid.Empty
                        ? await AddAsync(item)
                        : await UpdateAsync(item);

                results.Add(new Result<FlowInstanceData>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result<FlowInstanceData>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<FlowInstanceData> items)
    {
        foreach (FlowInstanceData item in items)
        {
            await DeleteAsync(item.Id);
        }
    }
}
