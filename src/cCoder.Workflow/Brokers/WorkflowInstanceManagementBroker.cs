using cCoder.Data;
using cCoder.Data.Models.Workflow;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Workflow.Brokers;

public interface IWorkflowInstanceManagementBroker
{
    object[] GetFailedExecutionStats();
    FlowInstanceData GetNextQueuedOrExecutingInstance(Guid flowDefinitionId);
    FlowInstanceData[] GetNextQueuedOrExecutingInstances();
    ValueTask<int> FlushOldInstancesAsync(DateTimeOffset cutoff, CancellationToken cancellationToken = default);
    ValueTask<FlowInstanceData> MarkExecutingAsync(Guid id, CancellationToken cancellationToken = default);
}

internal sealed class WorkflowInstanceManagementBroker(ICoreContextFactory coreContextFactory)
    : IWorkflowInstanceManagementBroker
{
    public object[] GetFailedExecutionStats()
    {
        using CoreDataContext core = coreContextFactory.CreateCoreContext();

        return core.FlowInstances
            .IgnoreQueryFilters()
            .Where(instance => instance.State == "Failed")
            .Include(instance => instance.FlowDefinition)
                .ThenInclude(definition => definition.App)
            .OrderByDescending(instance => instance.Start)
            .Select(instance => new
            {
                InstanceId = instance.Id,
                FlowId = instance.FlowDefinition.Id,
                Portal = instance.FlowDefinition.App.Domain,
                instance.Start,
                instance.End,
                AppName = instance.FlowDefinition.App.Name,
                FlowName = instance.FlowDefinition.Name
            })
            .Cast<object>()
            .ToArray();
    }

    public FlowInstanceData GetNextQueuedOrExecutingInstance(Guid flowDefinitionId)
    {
        using CoreDataContext core = coreContextFactory.CreateCoreContext();

        return core.FlowInstances
            .IgnoreQueryFilters()
            .Where(instance => instance.State == "Queued" || instance.State == "Executing")
            .Where(instance => instance.FlowDefinitionId == flowDefinitionId)
            .OrderBy(instance => instance.Start)
            .FirstOrDefault();
    }

    public FlowInstanceData[] GetNextQueuedOrExecutingInstances()
    {
        using CoreDataContext core = coreContextFactory.CreateCoreContext();

        return core.FlowInstances
            .Where(instance => instance.State == "Queued" || instance.State == "Executing")
            .Include(instance => instance.FlowDefinition)
                .ThenInclude(definition => definition.App)
            .GroupBy(instance => instance.FlowDefinitionId)
            .ToArray()
            .Select(group => group.OrderBy(instance => instance.Start).First())
            .ToArray();
    }

    public async ValueTask<int> FlushOldInstancesAsync(
        DateTimeOffset cutoff,
        CancellationToken cancellationToken = default)
    {
        using CoreDataContext core = coreContextFactory.CreateCoreContext();

        return await core.FlowInstances
            .IgnoreQueryFilters()
            .Where(instance => instance.Start < cutoff)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async ValueTask<FlowInstanceData> MarkExecutingAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        using CoreDataContext core = coreContextFactory.CreateCoreContext();

        FlowInstanceData instance = await core.FlowInstances
            .IgnoreQueryFilters()
            .Include(item => item.FlowDefinition)
                .ThenInclude(definition => definition.App)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (instance == null)
            return null;

        instance.Start = DateTimeOffset.UtcNow;
        instance.State = "Executing";

        _ = await core.SaveChangesAsync(cancellationToken);
        return instance;
    }
}
