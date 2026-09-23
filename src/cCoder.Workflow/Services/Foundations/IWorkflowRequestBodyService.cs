// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Services.Foundations;

internal interface IWorkflowRequestBodyService
{
    ValueTask<string> ReadTextAsync(Stream stream);
}