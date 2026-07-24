// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Foundations;

namespace cCoder.Workflow.Services.Processings;

internal class FlowInstanceDataProcessingService(IFlowInstanceDataService service)
    : IFlowInstanceDataProcessingService
{
    public FlowInstanceData Get(Guid id)
    {
        return service.Get(id:id);
    }

    public IQueryable<FlowInstanceData> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters:ignoreFilters);
    }

    public ValueTask<FlowInstanceData> AddAsync(FlowInstanceData entity)
    {
        return service.AddAsync(flowInstanceData:entity);
    }

    public ValueTask<FlowInstanceData> AddQueuedAsync(FlowInstanceData entity)
    {
        return service.AddQueuedAsync(flowInstanceData:entity);
    }

    public async ValueTask<FlowInstanceData> UpdateAsync(FlowInstanceData entity)
    {
        FlowInstanceData dbVersion = service.Get(id:entity.Id);
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
        return await service.UpdateAsync(flowInstanceData:dbVersion);
    }

    public ValueTask DeleteAsync(Guid id)
    {
        return service.DeleteAsync(id:id);
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
                        ? await AddAsync(entity:item)
                        : await UpdateAsync(entity:item);

                results.Add(item:new Result<FlowInstanceData>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item:new Result<FlowInstanceData>
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
            await DeleteAsync(id:item.Id);
        }
    }
}