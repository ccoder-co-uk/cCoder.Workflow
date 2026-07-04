using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Brokers.Storage;

public interface IScheduledTaskBroker
{
    IQueryable<ScheduledTask> GetAllScheduledTasks(bool ignoreFilters);
    ScheduledTask GetScheduledTaskForExecution(int id);
    ValueTask<ScheduledTask> MarkScheduledTaskExecutedAsync(int id, bool incrementNextExecution);
    bool ExecuteAsUserBelongsToApp(string executeAs, int appId);
    bool FlowBelongsToApp(Guid flowId, int appId);
    ValueTask<ScheduledTask> AddScheduledTaskAsync(ScheduledTask entity);
    ValueTask<ScheduledTask> UpdateScheduledTaskAsync(ScheduledTask entity);
    ValueTask<int> DeleteScheduledTaskAsync(ScheduledTask entity);
    ValueTask DeleteAllScheduledTasksAsync(IEnumerable<ScheduledTask> items);
    ValueTask DeleteAllScheduledTasksByAppIdAsync(int appId);
    int? GetAppId(ScheduledTask entity);
}







