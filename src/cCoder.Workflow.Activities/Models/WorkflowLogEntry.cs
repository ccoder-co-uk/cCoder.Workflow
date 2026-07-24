// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Newtonsoft.Json;


namespace cCoder.Workflow.Activities.Models;

public class WorkflowLogEntry
{
    public string Level { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public string Message { get; set; }

    public WorkflowLogEntry()
    {
        Timestamp = DateTimeOffset.UtcNow;
    }

    public WorkflowLogEntry(WorkflowLogLevel level, string message)
    {
        Level = level.ToString();
        Message = message;
        Timestamp = DateTimeOffset.UtcNow;
    }
}