using System.Security;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations;

namespace cCoder.Workflow.Services.Processings;

internal class ScheduledTaskProcessingService(
    IScheduledTaskService service,
    IAuthorizationBroker authorizationBroker,
    IScheduledTaskEventProcessingService scheduledTaskEventProcessingService) : IScheduledTaskProcessingService
{
    public ScheduledTask Get(int id)
    {
        return service.Get(id);
    }

    public IQueryable<ScheduledTask> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

    public async ValueTask ExecuteAsync(int id, bool incrementNextExecution = true)
    {
        ScheduledTask task = service.GetForExecution(id);
        if (task != null && authorizationBroker.IsAdminOfApp(task.AppId))
        {
            ScheduledTask updatedTask = await service.MarkExecutedAsync(id, incrementNextExecution);
            await scheduledTaskEventProcessingService.RaiseScheduledTaskExecuteEventAsync(updatedTask);
            return;
        }
        throw new SecurityException("Access Denied!");
    }

    public ValueTask<ScheduledTask> AddAsync(ScheduledTask entity)
    {
        if (!SecurityCheckTask(entity))
        {
            throw new SecurityException("Access Denied!");
        }
        return service.AddAsync(entity);
    }

    public ValueTask<ScheduledTask> UpdateAsync(ScheduledTask entity)
    {
        if (!SecurityCheckTask(entity))
        {
            throw new SecurityException("Access Denied!");
        }
        return service.UpdateAsync(entity);
    }

    public ValueTask DeleteAsync(int id)
    {
        return service.DeleteAsync(id);
    }

    public ValueTask DeleteByAppIdAsync(int appId) =>
        service.DeleteAllForAppAsync(
            service.GetAll(ignoreFilters: true)
                .Where(task => task.AppId == appId)
                .ToArray());

    public async ValueTask<IEnumerable<Result<ScheduledTask>>> AddOrUpdate(IEnumerable<ScheduledTask> items)
    {
        List<Result<ScheduledTask>> results = new List<Result<ScheduledTask>>();

        foreach (ScheduledTask item in items)
        {
            try
            {
                ScheduledTask savedItem =
                    item.Id == 0
                        ? await AddAsync(item)
                        : await UpdateAsync(item);

                results.Add(new Result<ScheduledTask>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result<ScheduledTask>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<ScheduledTask> items)
    {
        foreach (ScheduledTask item in items)
        {
            await DeleteAsync(item.Id);
        }
    }

    private bool SecurityCheckTask(ScheduledTask task)
    {
        bool flag = authorizationBroker.IsAdminOfApp(task.AppId);
        bool flag2 = service.ExecuteAsUserBelongsToApp(task.ExecuteAs, task.AppId);
        bool flag3 = service.FlowBelongsToApp(task.FlowId, task.AppId);
        return flag && flag2 && flag3;
    }
}
