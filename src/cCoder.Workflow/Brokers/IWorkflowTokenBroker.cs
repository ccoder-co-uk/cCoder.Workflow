// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;

namespace cCoder.Workflow.Brokers;

internal interface IWorkflowTokenBroker
{
    ValueTask<Token> IssueWorkflowExecutionTokenAsync(string userId);
}