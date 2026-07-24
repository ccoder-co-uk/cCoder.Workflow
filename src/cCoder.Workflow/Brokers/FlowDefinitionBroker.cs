// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Workflow;
using Microsoft.EntityFrameworkCore;


namespace cCoder.Workflow.Brokers;

public class FlowDefinitionBroker(ICoreContextFactory coreContextFactory)
    : IFlowDefinitionBroker
{

    public IQueryable<FlowDefinition> GetAllFlowDefinitions(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return ignoreFilters
            ? coreDataContext.FlowDefinitions.IgnoreQueryFilters()
            : coreDataContext.FlowDefinitions;
    }

    public async ValueTask<FlowDefinition> AddFlowDefinitionAsync(FlowDefinition entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        FlowDefinition result = (await coreDataContext.FlowDefinitions.AddAsync(entity: entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<FlowDefinition> UpdateFlowDefinitionAsync(FlowDefinition entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        FlowDefinition result = coreDataContext.FlowDefinitions.Update(entity: entity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteFlowDefinitionAsync(FlowDefinition entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.FlowDefinitions.Remove(entity: entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteFlowDefinitionWithInstancesAsync(Guid flowDefinitionId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        FlowDefinition flowDefinition = coreDataContext.FlowDefinitions
            .IgnoreQueryFilters()
            .Include(navigationPropertyPath: foundFlowDefinition => foundFlowDefinition.Instances)
            .FirstOrDefault(predicate: foundFlowDefinition => foundFlowDefinition.Id == flowDefinitionId);

        if (flowDefinition is null)
        {
            return;
        }

        coreDataContext.FlowInstances.RemoveRange(entities: flowDefinition.Instances);
        coreDataContext.FlowDefinitions.Remove(entity: flowDefinition);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteFlowDefinitionsWithInstancesByAppIdAsync(int appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        IQueryable<Guid> flowIds =
            coreDataContext.FlowDefinitions
                .IgnoreQueryFilters()
                .Where(predicate: flowDefinition => flowDefinition.AppId == appId)
                .Select(selector: flowDefinition => flowDefinition.Id);

        await coreDataContext.FlowInstances
            .IgnoreQueryFilters()
            .Where(predicate: instance => flowIds.Contains(item: instance.FlowDefinitionId))
            .ExecuteDeleteAsync();

        await coreDataContext.WorflowEvents
            .IgnoreQueryFilters()
            .Where(predicate: workflowEvent => flowIds.Contains(item: workflowEvent.FlowId))
            .ExecuteDeleteAsync();

        await coreDataContext.FlowDefinitions
            .IgnoreQueryFilters()
            .Where(predicate: flowDefinition => flowDefinition.AppId == appId)
            .ExecuteDeleteAsync();
    }

    public async ValueTask DeleteAllFlowDefinitionsAsync(IEnumerable<FlowDefinition> items)
    {
        if (items == null || !items.Any())
        {
            return;
        }

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.FlowDefinitions.RemoveRange(entities: items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(FlowDefinition entity)
    {
        return entity.AppId;
    }
}