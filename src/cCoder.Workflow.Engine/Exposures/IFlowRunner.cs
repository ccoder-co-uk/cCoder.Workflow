// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Engine.Exposures;

public interface IFlowRunner
{
    Task RunAsync(WorkflowRequest workflowRequest);
}