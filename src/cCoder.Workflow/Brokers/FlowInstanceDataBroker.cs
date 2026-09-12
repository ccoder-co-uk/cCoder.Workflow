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

    public async ValueTask<FlowInstanceData> AddFlowInstanceDataAsync(FlowInstanceData newFlowInstanceData)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        FlowInstanceData result = (await coreDataContext.FlowInstances.AddAsync(entity: newFlowInstanceData)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<FlowInstanceData> UpdateFlowInstanceDataAsync(FlowInstanceData updatedFlowInstanceData)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        FlowInstanceData result = coreDataContext.FlowInstances.Update(entity: updatedFlowInstanceData).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteFlowInstanceDataAsync(FlowInstanceData deletedFlowInstanceData)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.FlowInstances.Remove(entity: deletedFlowInstanceData);
        return await coreDataContext.SaveChangesAsync();
    }

    public int? SelectAppId(FlowInstanceData flowInstanceData)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.FlowDefinitions

            .Where(predicate: flowDefinition => flowDefinition.Id == flowInstanceData.FlowDefinitionId)
            .Select(selector: flowDefinition => (int?)flowDefinition.AppId)
            .FirstOrDefault();

    }
}