// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
    public ScheduledTask Get(int scheduledTaskId)
    {
        return service.Get(scheduledTaskId: scheduledTaskId);
    }

    public IQueryable<ScheduledTask> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters: ignoreFilters);
    }

    public async ValueTask ExecuteAsync(int scheduledTaskId, bool incrementNextExecution = true)
    {
        ScheduledTask task = service.GetForExecution(scheduledTaskId: scheduledTaskId);

        if (task != null && authorizationBroker.IsAdminOfApp(appId: task.AppId))
        {
            ScheduledTask updatedTask = await service.MarkExecutedAsync(scheduledTaskId: scheduledTaskId, incrementNextExecution: incrementNextExecution);
            await scheduledTaskEventProcessingService.RaiseScheduledTaskExecuteEventAsync(entity: updatedTask);
            return;
        }

        throw new SecurityException("Access Denied!");
    }

    public ValueTask<ScheduledTask> AddAsync(ScheduledTask entity)
    {
        if (!SecurityCheckTask(task: entity))
        {
            throw new SecurityException("Access Denied!");
        }

        return service.AddAsync(scheduledTask: entity);
    }

    public ValueTask<ScheduledTask> UpdateAsync(ScheduledTask entity)
    {
        if (!SecurityCheckTask(task: entity))
        {
            throw new SecurityException("Access Denied!");
        }

        return service.UpdateAsync(scheduledTask: entity);
    }

    public ValueTask DeleteAsync(int scheduledTaskId)
    {
        return service.DeleteAsync(scheduledTaskId: scheduledTaskId);
    }

    public ValueTask DeleteByAppIdAsync(int appId) =>
        service.DeleteAllByAppIdAsync(appId: appId);

    public async ValueTask<IEnumerable<Result<ScheduledTask>>> AddOrUpdate(IEnumerable<ScheduledTask> items)
    {
        List<Result<ScheduledTask>> results = new List<Result<ScheduledTask>>();

        foreach (ScheduledTask item in items)
        {
            try
            {
                ScheduledTask savedItem =
                    item.Id == 0
                        ? await AddAsync(entity: item)
                        : await UpdateAsync(entity: item);

                results.Add(item: new Result<ScheduledTask>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<ScheduledTask>
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
            await DeleteAsync(scheduledTaskId: item.Id);
        }
    }

    private bool SecurityCheckTask(ScheduledTask task)
    {
        bool flag = authorizationBroker.IsAdminOfApp(appId: task.AppId);
        bool flag2 = service.ExecuteAsUserBelongsToApp(executeAs: task.ExecuteAs, appId: task.AppId);
        bool flag3 = service.FlowBelongsToApp(flowId: task.FlowId, appId: task.AppId);
        return flag && flag2 && flag3;
    }
}