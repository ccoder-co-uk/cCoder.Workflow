using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Foundations.Events;

public interface IFlowInstanceDataEventService
{
    ValueTask RaiseFlowInstanceDataAddEventAsync(FlowInstanceData entity);
    ValueTask RaiseFlowInstanceDataUpdateEventAsync(FlowInstanceData entity);
    ValueTask RaiseFlowInstanceDataDeleteEventAsync(FlowInstanceData entity);
}








