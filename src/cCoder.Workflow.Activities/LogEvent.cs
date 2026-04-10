using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Activities;

public delegate Task LogEvent(WorkflowLogLevel level, string message);
