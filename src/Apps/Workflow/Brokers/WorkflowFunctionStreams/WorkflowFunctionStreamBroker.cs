// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Azure.Functions.Worker.Http;
using System.Text;

namespace Workflow.Brokers.WorkflowFunctionStreams;

internal sealed class WorkflowFunctionStreamBroker : IWorkflowFunctionStreamBroker
{
    public async ValueTask<string> ReadBodyAsync(HttpRequestData request)
    {
        using MemoryStream content = new();
        await request.Body.CopyToAsync(destination: content);

        return Encoding.UTF8.GetString(bytes: content.ToArray());
    }
}