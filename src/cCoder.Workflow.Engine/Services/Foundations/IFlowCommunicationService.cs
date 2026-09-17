// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Engine.Services.Foundations;

internal interface IFlowCommunicationService
{
    ValueTask ConnectWorkflowRequestAsync(
        WorkflowRequest workflowRequest);

    ValueTask LogWorkflowRequestAsync(
        WorkflowRequest workflowRequest,
        WorkflowLogLevel level,
        string message);
}