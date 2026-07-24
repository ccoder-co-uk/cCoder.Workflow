// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities;
using cCoder.Workflow.Activities.Activities;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Dependencies;

namespace cCoder.Workflow.Engine.Models;

public sealed class FlowExecution
{
    public WorkflowRequest Request { get; set; }

    public FlowInstanceData Result { get; set; }

    public Guid Id { get; set; }

    public int AppId { get; set; }

    public string Caller { get; set; }

    public string Name { get; set; }

    public Guid FlowDefinitionId { get; set; }

    public Flow Flow { get; set; }

    public WorkflowExecutionContext Context { get; set; }

    public DateTimeOffset Start { get; set; }

    public IScriptRunner Script { get; set; }

    public LogEvent Log { get; set; }
}