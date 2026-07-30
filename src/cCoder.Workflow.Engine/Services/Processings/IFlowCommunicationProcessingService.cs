// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Engine.Services.Processings;

internal interface IFlowCommunicationProcessingService
{
    ValueTask ConnectWorkflowRequestAsync(
        WorkflowRequest workflowRequest);

    ValueTask LogWorkflowRequestAsync(
        WorkflowRequest workflowRequest,
        WorkflowLogLevel level,
        string message);
}