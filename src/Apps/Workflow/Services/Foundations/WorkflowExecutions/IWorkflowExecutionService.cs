// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

namespace Workflow.Services.Foundations.WorkflowExecutions;

internal interface IWorkflowExecutionService
{
    Task RunWorkflowRequestAsync(string workflowRequestJson);
}