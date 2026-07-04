using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Services.Foundations;

public interface IScheduledTaskService
{
    ScheduledTask Get(int id);
    ScheduledTask GetForExecution(int id);
    IQueryable<ScheduledTask> GetAll(bool ignoreFilters = false);
    ValueTask<ScheduledTask> MarkExecutedAsync(int id, bool incrementNextExecution);
    bool ExecuteAsUserBelongsToApp(string executeAs, int appId);
    bool FlowBelongsToApp(Guid flowId, int appId);
    ValueTask<ScheduledTask> AddAsync(ScheduledTask scheduledTask);
    ValueTask<ScheduledTask> UpdateAsync(ScheduledTask scheduledTask);
    ValueTask DeleteAsync(int id);
    ValueTask DeleteAllForAppAsync(IEnumerable<ScheduledTask> items);
}










