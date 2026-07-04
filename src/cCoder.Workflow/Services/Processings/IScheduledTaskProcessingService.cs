using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Services.Processings;

public interface IScheduledTaskProcessingService
{
    ScheduledTask Get(int id);

    IQueryable<ScheduledTask> GetAll(bool ignoreFilters = false);

    ValueTask<ScheduledTask> AddAsync(ScheduledTask entity);

    ValueTask<ScheduledTask> UpdateAsync(ScheduledTask entity);

    ValueTask DeleteAsync(int id);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<Result<ScheduledTask>>> AddOrUpdate(IEnumerable<ScheduledTask> items);

    ValueTask DeleteAllAsync(IEnumerable<ScheduledTask> items);

    ValueTask ExecuteAsync(int id, bool incrementNextExecution = true);
}
