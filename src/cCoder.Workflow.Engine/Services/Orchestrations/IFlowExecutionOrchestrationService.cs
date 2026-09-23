// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Models;

namespace cCoder.Workflow.Engine.Services.Orchestrations;

internal interface IFlowExecutionOrchestrationService
{
    ValueTask<FlowExecution> ExecuteFlowExecutionAsync(
        FlowExecution flowExecution);
}