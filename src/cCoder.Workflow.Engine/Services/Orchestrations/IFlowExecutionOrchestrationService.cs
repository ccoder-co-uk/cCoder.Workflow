// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Engine.Services.Orchestrations;

public interface IFlowExecutionOrchestrationService
{
    Task ExecuteAsync(WorkflowRequest request);
}