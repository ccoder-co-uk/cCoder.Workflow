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

internal sealed partial class ScheduledTaskProcessingService(
    IScheduledTaskService service,
    IAuthorizationBroker authorizationBroker,
    IScheduledTaskEventProcessingService scheduledTaskEventProcessingService) : IScheduledTaskProcessingService
{
    public ScheduledTask Get(int scheduledTaskId) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [scheduledTaskId]); return ExecuteGet(scheduledTaskId: scheduledTaskId); });

    private ScheduledTask ExecuteGet(int scheduledTaskId)
    {
        return service.Get(scheduledTaskId: scheduledTaskId);
    }

    public IQueryable<ScheduledTask> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<ScheduledTask> ExecuteGetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask ExecuteAsync(int scheduledTaskId, bool incrementNextExecution = true) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [scheduledTaskId, incrementNextExecution]); await ExecuteExecuteAsync(scheduledTaskId: scheduledTaskId, incrementNextExecution: incrementNextExecution); }, isValueTask: true);

    private async ValueTask ExecuteExecuteAsync(int scheduledTaskId, bool incrementNextExecution = true)
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

    public ValueTask<ScheduledTask> AddScheduledTaskAsync(ScheduledTask newEntity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [newEntity]); return await ExecuteAddAsync(entity: newEntity); }, isValueTask: true);

    private ValueTask<ScheduledTask> ExecuteAddAsync(ScheduledTask entity)
    {
        if (!SecurityCheckTask(task: entity))
        {
            throw new SecurityException("Access Denied!");
        }

        return service.AddScheduledTaskAsync(newScheduledTask: entity);
    }

    public ValueTask<ScheduledTask> UpdateScheduledTaskAsync(ScheduledTask updatedEntity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [updatedEntity]); return await ExecuteUpdateAsync(entity: updatedEntity); }, isValueTask: true);

    private ValueTask<ScheduledTask> ExecuteUpdateAsync(ScheduledTask entity)
    {
        if (!SecurityCheckTask(task: entity))
        {
            throw new SecurityException("Access Denied!");
        }

        return service.UpdateScheduledTaskAsync(updatedScheduledTask: entity);
    }

    public ValueTask DeleteAsync(int scheduledTaskId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [scheduledTaskId]); await ExecuteDeleteAsync(scheduledTaskId: scheduledTaskId); }, isValueTask: true);

    private ValueTask ExecuteDeleteAsync(int scheduledTaskId)
    {
        return service.DeleteAsync(scheduledTaskId: scheduledTaskId);
    }

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [appId]); await ExecuteDeleteByAppIdAsync(appId: appId); }, isValueTask: true);

    private ValueTask ExecuteDeleteByAppIdAsync(int appId) =>
        service.DeleteAllByAppIdAsync(appId: appId);

    public ValueTask<IEnumerable<Result<ScheduledTask>>> AddOrUpdateScheduledTask(IEnumerable<ScheduledTask> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); return await ExecuteAddOrUpdate(items: items); }, isValueTask: true);

    private async ValueTask<IEnumerable<Result<ScheduledTask>>> ExecuteAddOrUpdate(IEnumerable<ScheduledTask> items)
    {
        List<Result<ScheduledTask>> results = new List<Result<ScheduledTask>>();

        foreach (ScheduledTask item in items)
        {
            try
            {
                ScheduledTask savedItem =
                    item.Id == 0
                        ? await AddScheduledTaskAsync(newEntity: item)
                        : await UpdateScheduledTaskAsync(updatedEntity: item);

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

    public ValueTask DeleteAllScheduledTaskAsync(IEnumerable<ScheduledTask> deletedItems) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [deletedItems]); await ExecuteDeleteAllAsync(items: deletedItems); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAllAsync(IEnumerable<ScheduledTask> items)
    {
        foreach (ScheduledTask item in items)
        {
            await DeleteAsync(scheduledTaskId: item.Id);
        }
    }

    private bool SecurityCheckTask(ScheduledTask task)
    {
        bool flag = authorizationBroker.IsAdminOfApp(appId: task.AppId);
        bool flag2 = service.GetExecuteAsUserBelongsToApp(executeAs: task.ExecuteAs, appId: task.AppId);
        bool flag3 = service.GetFlowBelongsToApp(flowId: task.FlowId, appId: task.AppId);
        return flag && flag2 && flag3;
    }
}