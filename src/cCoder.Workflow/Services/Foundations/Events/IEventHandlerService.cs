namespace cCoder.Workflow.Services.Foundations.Events;

public interface IEventHandlerService
{
    void ListenToAllEvents();
    void ListenToScheduledTaskExecuteEvents();
}

