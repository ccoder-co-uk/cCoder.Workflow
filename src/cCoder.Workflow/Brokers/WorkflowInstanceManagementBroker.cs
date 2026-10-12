// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using cCoder.Data;
using cCoder.Data.Models.Workflow;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Workflow.Brokers;

public interface IWorkflowInstanceManagementBroker
{
    ValueTask<int> CompareExchangeInstanceAsync(
        cCoder.Workflow.Models.WorkflowInstanceExchange workflowInstanceExchange,
        CancellationToken cancellationToken = default);

    FlowInstanceData[] SelectExpiredInstances(DateTimeOffset cutoff);

    object[] GetFailedExecutionStats();

    FlowInstanceData[] GetQueuedInstances();

    ValueTask<int> FlushOldInstancesAsync(DateTimeOffset cutoff, CancellationToken cancellationToken = default);

    ValueTask<int> RequeueHungExecutingInstancesAsync(DateTimeOffset cutoff, CancellationToken cancellationToken = default);

    ValueTask<int> UpdateQueuedInstanceClaimAsync(
        Guid flowInstanceDataId,
        CancellationToken cancellationToken = default);

    ValueTask<FlowInstanceData> SelectClaimedInstanceAsync(
        Guid flowInstanceDataId,
        CancellationToken cancellationToken = default);

    ValueTask<int> MarkInstanceFailedAsync(Guid flowInstanceDataId, DateTimeOffset failedAt, CancellationToken cancellationToken = default);
}

internal sealed class WorkflowInstanceManagementBroker(ICoreContextFactory coreContextFactory)
    : IWorkflowInstanceManagementBroker
{
    public async ValueTask<int> CompareExchangeInstanceAsync(
        cCoder.Workflow.Models.WorkflowInstanceExchange workflowInstanceExchange,
        CancellationToken cancellationToken = default)
    {
        using CoreDataContext core = coreContextFactory.CreateCoreContext();

        return await core.FlowInstances
            .IgnoreQueryFilters()
            .Where(predicate: instance => instance.Id == workflowInstanceExchange.Expected.Id
                && instance.FlowDefinitionId == workflowInstanceExchange.Expected.FlowDefinitionId
                && instance.Caller == workflowInstanceExchange.Expected.Caller
                && instance.State == workflowInstanceExchange.Expected.State
                && instance.Start == workflowInstanceExchange.Expected.Start
                && instance.End == workflowInstanceExchange.Expected.End
                && instance.ContextJson == workflowInstanceExchange.Expected.ContextJson)
            .ExecuteUpdateAsync(setPropertyCalls: setters => setters
                .SetProperty(propertyExpression: instance => instance.ContextJson, valueExpression: workflowInstanceExchange.Replacement.ContextJson)
                .SetProperty(propertyExpression: instance => instance.State, valueExpression: workflowInstanceExchange.Replacement.State)
                .SetProperty(propertyExpression: instance => instance.Start, valueExpression: workflowInstanceExchange.Replacement.Start)
                .SetProperty(propertyExpression: instance => instance.End, valueExpression: workflowInstanceExchange.Replacement.End),
                cancellationToken: cancellationToken);
    }

    public FlowInstanceData[] SelectExpiredInstances(DateTimeOffset cutoff)
    {
        using CoreDataContext core = coreContextFactory.CreateCoreContext();

        return core.FlowInstances.IgnoreQueryFilters()
            .AsNoTracking()
            .Where(predicate: instance => (instance.State == "AwaitingExecution" || instance.State == "Executing") && instance.Start <= cutoff)
            .OrderBy(keySelector: instance => instance.Start)
            .Take(count: 100)
            .ToArray();
    }

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

    public async ValueTask<int> UpdateQueuedInstanceClaimAsync(
        Guid flowInstanceDataId,
        CancellationToken cancellationToken = default)
    {
        using CoreDataContext core = coreContextFactory.CreateCoreContext();

        DateTimeOffset claimedAt = DateTimeOffset.UtcNow;

        return await core.FlowInstances
            .IgnoreQueryFilters()
            .Where(predicate: instance => instance.Id == flowInstanceDataId)
            .Where(predicate: instance => instance.State == "Queued")
            .ExecuteUpdateAsync(
setPropertyCalls: setters => setters
                    .SetProperty(propertyExpression: instance => instance.State, valueExpression: "AwaitingExecution")
                    .SetProperty(propertyExpression: instance => instance.Start, valueExpression: claimedAt)
                    .SetProperty(propertyExpression: instance => instance.End, valueExpression: (DateTimeOffset?)null),
cancellationToken: cancellationToken);
    }

    public async ValueTask<FlowInstanceData> SelectClaimedInstanceAsync(
        Guid flowInstanceDataId,
        CancellationToken cancellationToken = default)
    {
        using CoreDataContext core = coreContextFactory.CreateCoreContext();

        return await core.FlowInstances
            .IgnoreQueryFilters()
            .Include(navigationPropertyPath: item => item.FlowDefinition)
                .ThenInclude(navigationPropertyPath: definition => definition.App)
            .FirstOrDefaultAsync(predicate: item => item.Id == flowInstanceDataId, cancellationToken: cancellationToken);
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