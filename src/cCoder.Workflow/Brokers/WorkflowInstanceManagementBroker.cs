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

    ValueTask<FlowInstanceData> ClaimQueuedInstanceAsync(Guid flowInstanceDataId, CancellationToken cancellationToken = default);

    ValueTask<int> MarkInstanceFailedAsync(Guid flowInstanceDataId, DateTimeOffset failedAt, CancellationToken cancellationToken = default);
}

internal sealed class WorkflowInstanceManagementBroker(ICoreContextFactory coreContextFactory)
    : IWorkflowInstanceManagementBroker
{
    public object[] GetFailedExecutionStats()
    {
        using CoreDataContext core = coreContextFactory.CreateCoreContext();

        return core.FlowInstances
            .IgnoreQueryFilters()
            .Where(predicate: instance => instance.State == "Failed")
            .Include(navigationPropertyPath: instance => instance.FlowDefinition)
                .ThenInclude(navigationPropertyPath: definition => definition.App)
            .OrderByDescending(keySelector: instance => instance.Start)
            .Select(selector: instance => new
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
            .Where(predicate: instance => instance.State == "Queued")
            .Include(navigationPropertyPath: instance => instance.FlowDefinition)
                .ThenInclude(navigationPropertyPath: definition => definition.App)
            .OrderBy(keySelector: instance => instance.Start)
            .ToArray();
    }

    public async ValueTask<int> FlushOldInstancesAsync(
        DateTimeOffset cutoff,
        CancellationToken cancellationToken = default)
    {
        using CoreDataContext core = coreContextFactory.CreateCoreContext();

        return await core.FlowInstances
            .IgnoreQueryFilters()
            .Where(predicate: instance => instance.Start < cutoff)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);
    }

    public async ValueTask<int> RequeueHungExecutingInstancesAsync(
        DateTimeOffset cutoff,
        CancellationToken cancellationToken = default)
    {
        using CoreDataContext core = coreContextFactory.CreateCoreContext();

        return await core.FlowInstances
            .IgnoreQueryFilters()
            .Where(predicate: instance => instance.State == "Executing")
            .Where(predicate: instance => instance.Start < cutoff)
            .ExecuteUpdateAsync(
setPropertyCalls: setters => setters
                    .SetProperty(propertyExpression: instance => instance.State, valueExpression: "Queued")
                    .SetProperty(propertyExpression: instance => instance.End, valueExpression: (DateTimeOffset?)null),
cancellationToken: cancellationToken);
    }

    public async ValueTask<FlowInstanceData> ClaimQueuedInstanceAsync(
        Guid flowInstanceDataId,
        CancellationToken cancellationToken = default)
    {
        using CoreDataContext core = coreContextFactory.CreateCoreContext();

        DateTimeOffset claimedAt = DateTimeOffset.UtcNow;

        int claimedCount = await core.FlowInstances
            .IgnoreQueryFilters()
            .Where(predicate: instance => instance.Id == flowInstanceDataId)
            .Where(predicate: instance => instance.State == "Queued")
            .ExecuteUpdateAsync(
setPropertyCalls: setters => setters
                    .SetProperty(propertyExpression: instance => instance.State, valueExpression: "Executing")
                    .SetProperty(propertyExpression: instance => instance.Start, valueExpression: claimedAt)
                    .SetProperty(propertyExpression: instance => instance.End, valueExpression: (DateTimeOffset?)null),
cancellationToken: cancellationToken);

        if (claimedCount == 0)
        {
            return null;
        }

        FlowInstanceData instance = await core.FlowInstances
            .IgnoreQueryFilters()
            .Include(navigationPropertyPath: item => item.FlowDefinition)
                .ThenInclude(navigationPropertyPath: definition => definition.App)
            .FirstOrDefaultAsync(predicate: item => item.Id == flowInstanceDataId, cancellationToken: cancellationToken);

        if (instance == null)
        {
            return null;
        }

        return instance;
    }

    public async ValueTask<int> MarkInstanceFailedAsync(
        Guid flowInstanceDataId,
        DateTimeOffset failedAt,
        CancellationToken cancellationToken = default)
    {
        using CoreDataContext core = coreContextFactory.CreateCoreContext();

        return await core.FlowInstances
            .IgnoreQueryFilters()
            .Where(predicate: instance => instance.Id == flowInstanceDataId)
            .ExecuteUpdateAsync(
setPropertyCalls: setters => setters
                    .SetProperty(propertyExpression: instance => instance.State, valueExpression: "Failed")
                    .SetProperty(propertyExpression: instance => instance.End, valueExpression: failedAt),
cancellationToken: cancellationToken);
    }
}