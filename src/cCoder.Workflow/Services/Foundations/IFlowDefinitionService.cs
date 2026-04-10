using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Foundations;

public interface IFlowDefinitionService
{
    FlowDefinition Get(Guid id);
    IQueryable<FlowDefinition> GetAll(bool ignoreFilters = false);
    ValueTask<FlowDefinition> AddAsync(FlowDefinition flowDefinition);
    ValueTask<FlowDefinition> UpdateAsync(FlowDefinition flowDefinition);
    ValueTask DeleteAsync(Guid id);
    ValueTask DeleteWithInstancesAsync(Guid id);
}








