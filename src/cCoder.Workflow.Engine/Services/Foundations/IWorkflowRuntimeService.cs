// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.Workflow.Engine.Models;

namespace cCoder.Workflow.Engine.Services.Foundations;

internal interface IWorkflowRuntimeService
{
    ValueTask<FlowExecution> ExecuteFlowExecutionAsync(
        FlowExecution flowExecution);
}