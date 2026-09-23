// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;
using cCoder.Workflow.Brokers;

namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class WorkflowTokenService(IWorkflowTokenBroker workflowTokenBroker)
    : IWorkflowTokenService
{
    public ValueTask<Token> IssueWorkflowExecutionTokenAsync(string userId) =>
        TryCatch(
            operation: async () =>
            {
                ValidateWorkflowExecutionTokenOnIssue(inputs: [userId]);
                return await workflowTokenBroker.IssueWorkflowExecutionTokenAsync(userId: userId);
            },
            isValueTask: true);
}