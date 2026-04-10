using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Activities;

public interface IWorkflowContext
{
    IScriptRunner Script { get; }

    IDictionary<string, object> Variables { get; }

    Flow Flow { get; }

    void Log(WorkflowLogLevel level, string message);
}
