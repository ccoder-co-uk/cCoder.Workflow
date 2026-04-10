using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Foundations;

public interface IFlowInstanceDataService
{
    FlowInstanceData Get(Guid id);
    IQueryable<FlowInstanceData> GetAll(bool ignoreFilters = false);
    ValueTask<FlowInstanceData> AddAsync(FlowInstanceData flowInstanceData);
    ValueTask<FlowInstanceData> UpdateAsync(FlowInstanceData flowInstanceData);
    ValueTask DeleteAsync(Guid id);
}








