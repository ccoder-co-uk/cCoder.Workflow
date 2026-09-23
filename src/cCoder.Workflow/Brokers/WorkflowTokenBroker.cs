// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Exposures;
using cCoder.Security.Models.Entities;

namespace cCoder.Workflow.Brokers;

internal sealed class WorkflowTokenBroker(ITokenManager tokenManager)
    : IWorkflowTokenBroker
{
    public ValueTask<Token> IssueWorkflowExecutionTokenAsync(string userId) =>
        tokenManager.IssueTokenAsync(
            userId: userId,
            tokenUse: TokenUse.WorkflowExecution);
}