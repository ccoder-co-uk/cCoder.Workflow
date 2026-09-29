// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.IO;
using System.Threading.Tasks;

namespace cCoder.Workflow.Services.Foundations;

internal interface IWorkflowRequestBodyService
{
    ValueTask<string> ReadTextAsync(Stream stream);
}