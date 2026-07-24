// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Models;

namespace cCoder.Workflow.Engine.Services.Processings;

public interface IFlowInstanceProcessingService
{
    ValueTask<FlowExecution> ExecuteFlowExecutionAsync(
        FlowExecution flowExecution);
}