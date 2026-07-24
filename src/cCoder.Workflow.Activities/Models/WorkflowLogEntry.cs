// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;


namespace cCoder.Workflow.Activities.Models;

public class WorkflowLogEntry
{
    [Required]
    public string Level { get; set; }

    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    [Required]
    public string Message { get; set; }

    public WorkflowLogEntry() { }

    public WorkflowLogEntry(WorkflowLogLevel level, string message)
    {
        Level = level.ToString();
        Message = message;
    }
}