// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.Security.Models.Entities;

namespace cCoder.Workflow.Services.Foundations;

internal interface IWorkflowTokenService
{
    ValueTask<Token> IssueWorkflowExecutionTokenAsync(string userId);
}