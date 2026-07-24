// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Workflow;
using Microsoft.EntityFrameworkCore;


namespace cCoder.Workflow.Brokers;

public class FlowInstanceDataBroker(ICoreContextFactory coreContextFactory) 
    : IFlowInstanceDataBroker
{

    public IQueryable<FlowInstanceData> GetAllFlowInstanceData(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return ignoreFilters
            ? coreDataContext.FlowInstances.IgnoreQueryFilters()
            : coreDataContext.FlowInstances;
    }

    public async ValueTask<FlowInstanceData> AddFlowInstanceDataAsync(FlowInstanceData entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        FlowInstanceData result = (await coreDataContext.FlowInstances.AddAsync(entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<FlowInstanceData> UpdateFlowInstanceDataAsync(FlowInstanceData entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        FlowInstanceData result = coreDataContext.FlowInstances.Update(entity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteFlowInstanceDataAsync(FlowInstanceData entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.FlowInstances.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllFlowInstanceDataAsync(IEnumerable<FlowInstanceData> items)
    {
        if (items == null || !items.Any())
            return;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.FlowInstances.RemoveRange(items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(FlowInstanceData entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return coreDataContext.FlowDefinitions

            .Where(flowDefinition => flowDefinition.Id == entity.FlowDefinitionId)
            .Select(flowDefinition => (int?)flowDefinition.AppId)
            .FirstOrDefault();

    }
}