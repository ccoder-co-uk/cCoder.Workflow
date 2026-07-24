// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;


namespace cCoder.Workflow.Brokers.Storage;

internal sealed class ScheduledTaskBroker(ICoreContextFactory coreContextFactory) : IScheduledTaskBroker
{

    public IQueryable<ScheduledTask> SelectAllScheduledTasks() =>
        coreContextFactory.CreateCoreContext().ScheduledTasks;

    public IQueryable<ScheduledTask> SelectAllScheduledTasksIgnoringQueryFilters() =>
        coreContextFactory.CreateCoreContext()
            .ScheduledTasks
            .IgnoreQueryFilters();

    public ScheduledTask SelectScheduledTaskForExecution(int scheduledTaskId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.ScheduledTasks
            .Include(navigationPropertyPath: task => task.ExecuteAsUser)
            .Include(navigationPropertyPath: task => task.Flow)
            .FirstOrDefault(predicate: task => task.Id == scheduledTaskId);
    }

    public bool SelectExecuteAsUserBelongsToApp(string executeAs, int appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Users
            .Any(predicate: user => user.Id == executeAs && user.Roles.Any(predicate: role => role.Role.AppId == appId));
    }

    public bool SelectFlowBelongsToApp(Guid flowId, int appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.FlowDefinitions.Any(predicate: flow => flow.Id == flowId && flow.AppId == appId);
    }

    public async ValueTask<ScheduledTask> InsertScheduledTaskAsync(ScheduledTask newEntity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        ScheduledTask result = (await coreDataContext.ScheduledTasks.AddAsync(entity: newEntity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<ScheduledTask> UpdateScheduledTaskAsync(ScheduledTask updatedEntity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        ScheduledTask result = coreDataContext.ScheduledTasks.Update(entity: updatedEntity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteScheduledTaskAsync(ScheduledTask deletedEntity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.ScheduledTasks.Remove(entity: deletedEntity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllScheduledTasksAsync(IEnumerable<ScheduledTask> deletedItems)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.ScheduledTasks.RemoveRange(entities: deletedItems);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllScheduledTasksByAppIdAsync(int appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        await coreDataContext.ScheduledTasks
            .IgnoreQueryFilters()
            .Where(predicate: task => task.AppId == appId)
            .ExecuteDeleteAsync();
    }

    public int? SelectAppId(ScheduledTask entity)
    {
        return entity.AppId;
    }
}