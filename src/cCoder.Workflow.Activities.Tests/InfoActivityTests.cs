// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Activities;
using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Activities.Tests;

public sealed partial class InfoActivityTests
{
    private sealed class TestWorkflowContext : IWorkflowContext
    {
        public Guid InstanceId { get; set; } = Guid.NewGuid();
        public Flow Flow { get; set; }
        public List<WorkflowLogEntry> ExecutionLog { get; set; } = [];
        public IDictionary<string, object> Variables { get; set; } = new Dictionary<string, object>
        {
            ["Imports"] = Activity.ScriptImports,
        };
        public string ExecutionState { get; set; }
        public IScriptRunner Script => null;

        public void Log(WorkflowLogLevel level, string message) =>
            ExecutionLog.Add(
                item: new WorkflowLogEntry(
                    level: level,
                    message: message));
    }

    private static InfoActivity CreateInfoActivity() =>
        new()
        {
            Ref = "info",
            Message = "hello",
            Previous = [],
            Next = [],
        };
}
