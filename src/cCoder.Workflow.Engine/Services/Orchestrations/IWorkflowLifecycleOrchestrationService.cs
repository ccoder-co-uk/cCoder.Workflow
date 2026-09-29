// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

using cCoder.Workflow.Engine.Models;

namespace cCoder.Workflow.Engine.Services.Orchestrations;

internal interface IWorkflowLifecycleOrchestrationService
{
    ValueTask StartFlowExecutionAsync(FlowExecution flowExecution);

    ValueTask SaveFlowExecutionAsync(FlowExecution flowExecution);

    ValueTask RecordFlowExecutionFailureAsync(
        FlowExecution flowExecution,
        Exception exception);

    ValueTask FinishFlowExecutionAsync(FlowExecution flowExecution);
}