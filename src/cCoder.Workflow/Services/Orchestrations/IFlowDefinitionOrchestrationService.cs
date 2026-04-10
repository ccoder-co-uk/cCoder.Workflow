using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Services.Orchestrations;

public interface IFlowDefinitionOrchestrationService
{
    FlowDefinition Get(Guid id);

    IQueryable<FlowDefinition> GetAll(bool ignoreFilters = false);

    ValueTask<FlowDefinition> AddAsync(FlowDefinition entity);

    ValueTask<FlowDefinition> UpdateAsync(FlowDefinition entity);

    ValueTask<Guid> QueueAsync(Guid id, string args);
    ValueTask<Guid> QueueAsync(Guid id, string asUserId, string args);

    ValueTask<Guid> QueueAsync(Guid id, User asUser, string args);

    ValueTask DeleteAsync(Guid id);
    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<Result<FlowDefinition>>> AddOrUpdate(IEnumerable<FlowDefinition> items);

    ValueTask DeleteAllAsync(IEnumerable<FlowDefinition> items);
}
