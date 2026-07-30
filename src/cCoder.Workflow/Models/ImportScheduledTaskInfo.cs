// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Models;

internal sealed class ImportScheduledTaskInfo
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string FlowName { get; set; }
    public string ExecuteAs { get; set; }
    public string ExecutionArgs { get; set; }
    public long ScheduleInTicks { get; set; }
    public DateTimeOffset? NextExecution { get; set; }
}