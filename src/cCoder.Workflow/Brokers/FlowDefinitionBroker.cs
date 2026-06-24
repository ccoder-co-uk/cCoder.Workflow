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
        FlowDefinition result = (await coreDataContext.FlowDefinitions.AddAsync(entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<FlowDefinition> UpdateFlowDefinitionAsync(FlowDefinition entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        FlowDefinition result = coreDataContext.FlowDefinitions.Update(entity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteFlowDefinitionAsync(FlowDefinition entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.FlowDefinitions.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteFlowDefinitionWithInstancesAsync(Guid id)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        FlowDefinition flowDefinition = coreDataContext.FlowDefinitions
            .IgnoreQueryFilters()
            .Include(foundFlowDefinition => foundFlowDefinition.Instances)
            .FirstOrDefault(foundFlowDefinition => foundFlowDefinition.Id == id);

        if (flowDefinition is null)
            return;

        coreDataContext.FlowInstances.RemoveRange(flowDefinition.Instances);
        coreDataContext.FlowDefinitions.Remove(flowDefinition);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllFlowDefinitionsAsync(IEnumerable<FlowDefinition> items)
    {
        if (items == null || !items.Any())
            return;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.FlowDefinitions.RemoveRange(items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(FlowDefinition entity)
    {
        return entity.AppId;
    }
}







