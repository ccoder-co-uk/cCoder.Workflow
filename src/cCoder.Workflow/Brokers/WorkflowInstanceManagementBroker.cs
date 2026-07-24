// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Workflow;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Workflow.Brokers;

public interface IWorkflowInstanceManagementBroker
{
    object[] GetFailedExecutionStats();
    FlowInstanceData[] GetQueuedInstances();
    ValueTask<int> FlushOldInstancesAsync(DateTimeOffset cutoff, CancellationToken cancellationToken = default);
    ValueTask<int> RequeueHungExecutingInstancesAsync(DateTimeOffset cutoff, CancellationToken cancellationToken = default);
    ValueTask<FlowInstanceData> ClaimQueuedInstanceAsync(Guid id, CancellationToken cancellationToken = default);
    ValueTask<int> MarkInstanceFailedAsync(Guid id, DateTimeOffset failedAt, CancellationToken cancellationToken = default);
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

    public FlowInstanceData[] GetQueuedInstances()
    {
        using CoreDataContext core = coreContextFactory.CreateCoreContext();

        return core.FlowInstances
            .IgnoreQueryFilters()
            .Where(instance => instance.State == "Queued")
            .Include(instance => instance.FlowDefinition)
                .ThenInclude(definition => definition.App)
            .OrderBy(instance => instance.Start)
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

    public async ValueTask<int> RequeueHungExecutingInstancesAsync(
        DateTimeOffset cutoff,
        CancellationToken cancellationToken = default)
    {
        using CoreDataContext core = coreContextFactory.CreateCoreContext();

        return await core.FlowInstances
            .IgnoreQueryFilters()
            .Where(instance => instance.State == "Executing")
            .Where(instance => instance.Start < cutoff)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(instance => instance.State, "Queued")
                    .SetProperty(instance => instance.End, (DateTimeOffset?)null),
                cancellationToken);
    }

    public async ValueTask<FlowInstanceData> ClaimQueuedInstanceAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        using CoreDataContext core = coreContextFactory.CreateCoreContext();

        DateTimeOffset claimedAt = DateTimeOffset.UtcNow;

        int claimedCount = await core.FlowInstances
            .IgnoreQueryFilters()
            .Where(instance => instance.Id == id)
            .Where(instance => instance.State == "Queued")
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(instance => instance.State, "Executing")
                    .SetProperty(instance => instance.Start, claimedAt)
                    .SetProperty(instance => instance.End, (DateTimeOffset?)null),
                cancellationToken);

        if (claimedCount == 0)
            return null;

        FlowInstanceData instance = await core.FlowInstances
            .IgnoreQueryFilters()
            .Include(item => item.FlowDefinition)
                .ThenInclude(definition => definition.App)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (instance == null)
            return null;

        return instance;
    }

    public async ValueTask<int> MarkInstanceFailedAsync(
        Guid id,
        DateTimeOffset failedAt,
        CancellationToken cancellationToken = default)
    {
        using CoreDataContext core = coreContextFactory.CreateCoreContext();

        return await core.FlowInstances
            .IgnoreQueryFilters()
            .Where(instance => instance.Id == id)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(instance => instance.State, "Failed")
                    .SetProperty(instance => instance.End, failedAt),
                cancellationToken);
    }
}