using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Services.Processings;

public interface IWorkflowEventProcessingService
{
    WorkflowEvent Get(Guid id);

    IQueryable<WorkflowEvent> GetAll(bool ignoreFilters = false);

    ValueTask<WorkflowEvent[]> GetSubscriptionsAsync(int appId, string eventContext);

    ValueTask<WorkflowEvent> AddAsync(WorkflowEvent entity);

    ValueTask<WorkflowEvent> UpdateAsync(WorkflowEvent entity);

    ValueTask DeleteAsync(Guid id);

    ValueTask<IEnumerable<Result<WorkflowEvent>>> AddOrUpdate(IEnumerable<WorkflowEvent> items);

    ValueTask DeleteAllAsync(IEnumerable<WorkflowEvent> items);
}
