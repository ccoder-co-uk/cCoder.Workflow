// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Workflow;
using Microsoft.EntityFrameworkCore;


namespace cCoder.Workflow.Brokers;

internal sealed class FlowInstanceDataBroker(ICoreContextFactory coreContextFactory)
    : IFlowInstanceDataBroker
{

    public IQueryable<FlowInstanceData> SelectAllFlowInstanceData() =>
        coreContextFactory.CreateCoreContext().FlowInstances;

    public IQueryable<FlowInstanceData> SelectAllFlowInstanceDataIgnoringQueryFilters() =>
        coreContextFactory.CreateCoreContext()
            .FlowInstances
            .IgnoreQueryFilters();

    public async ValueTask<FlowInstanceData> AddFlowInstanceDataAsync(FlowInstanceData entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        FlowInstanceData result = (await coreDataContext.FlowInstances.AddAsync(entity: entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<FlowInstanceData> UpdateFlowInstanceDataAsync(FlowInstanceData entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        FlowInstanceData result = coreDataContext.FlowInstances.Update(entity: entity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteFlowInstanceDataAsync(FlowInstanceData entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.FlowInstances.Remove(entity: entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public int? SelectAppId(FlowInstanceData entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.FlowDefinitions

            .Where(predicate: flowDefinition => flowDefinition.Id == entity.FlowDefinitionId)
            .Select(selector: flowDefinition => (int?)flowDefinition.AppId)
            .FirstOrDefault();

    }
}