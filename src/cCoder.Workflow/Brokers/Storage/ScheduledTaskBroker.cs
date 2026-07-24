// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;


namespace cCoder.Workflow.Brokers.Storage;

public class ScheduledTaskBroker(ICoreContextFactory coreContextFactory) : IScheduledTaskBroker
{

    public IQueryable<ScheduledTask> GetAllScheduledTasks(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return ignoreFilters
            ? coreDataContext.ScheduledTasks.IgnoreQueryFilters()
            : coreDataContext.ScheduledTasks;
    }

    public ScheduledTask GetScheduledTaskForExecution(int id)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.ScheduledTasks
            .Include(navigationPropertyPath:task => task.ExecuteAsUser)
            .Include(navigationPropertyPath:task => task.Flow)
            .FirstOrDefault(predicate:task => task.Id == id);
    }

    public async ValueTask<ScheduledTask> MarkScheduledTaskExecutedAsync(int id, bool incrementNextExecution)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        ScheduledTask task = coreDataContext.ScheduledTasks
            .IgnoreQueryFilters()
            .Include(navigationPropertyPath:foundTask => foundTask.ExecuteAsUser)
            .Include(navigationPropertyPath:foundTask => foundTask.Flow)
            .FirstOrDefault(predicate:foundTask => foundTask.Id == id);

        if (task is null)
            return null;

        task.LastExecuted = DateTimeOffset.UtcNow;

        if (incrementNextExecution)
            while (task.NextExecution < DateTimeOffset.UtcNow && task.NextExecution != null)
                task.NextExecution = task.ScheduleInTicks > 0
                    ? task.NextExecution + TimeSpan.FromTicks(value:task.ScheduleInTicks)
                    : null;

        _ = await coreDataContext.SaveChangesAsync();
        return task;
    }

    public bool ExecuteAsUserBelongsToApp(string executeAs, int appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.Users
            .Any(predicate:user => user.Id == executeAs && user.Roles.Any(role => role.Role.AppId == appId));
    }

    public bool FlowBelongsToApp(Guid flowId, int appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.FlowDefinitions.Any(predicate:flow => flow.Id == flowId && flow.AppId == appId);
    }

    public async ValueTask<ScheduledTask> AddScheduledTaskAsync(ScheduledTask entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        ScheduledTask result = (await coreDataContext.ScheduledTasks.AddAsync(entity:entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<ScheduledTask> UpdateScheduledTaskAsync(ScheduledTask entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        ScheduledTask result = coreDataContext.ScheduledTasks.Update(entity:entity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteScheduledTaskAsync(ScheduledTask entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.ScheduledTasks.Remove(entity:entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllScheduledTasksAsync(IEnumerable<ScheduledTask> items)
    {
        if (items == null || !items.Any())
            return;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.ScheduledTasks.RemoveRange(entities:items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllScheduledTasksByAppIdAsync(int appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        await coreDataContext.ScheduledTasks
            .IgnoreQueryFilters()
            .Where(predicate:task => task.AppId == appId)
            .ExecuteDeleteAsync();
    }

    public int? GetAppId(ScheduledTask entity)
    {
        return entity.AppId;
    }
}