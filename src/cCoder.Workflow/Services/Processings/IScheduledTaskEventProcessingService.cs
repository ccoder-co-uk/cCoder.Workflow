using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Processings;

public interface IScheduledTaskEventProcessingService
{
    ValueTask RaiseScheduledTaskAddEventAsync(ScheduledTask entity);
    ValueTask RaiseScheduledTaskUpdateEventAsync(ScheduledTask entity);
    ValueTask RaiseScheduledTaskDeleteEventAsync(ScheduledTask entity);
    ValueTask RaiseScheduledTaskExecuteEventAsync(ScheduledTask entity);
}









