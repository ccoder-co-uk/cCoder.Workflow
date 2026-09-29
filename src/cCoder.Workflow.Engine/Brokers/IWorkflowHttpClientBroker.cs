// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.Workflow.Engine.Models;

namespace cCoder.Workflow.Engine.Brokers;

internal interface IWorkflowHttpClientBroker
{
    ValueTask<string> GetStringAsync(
        string apiRoot,
        string authToken,
        string requestUri);

    ValueTask<WorkflowHttpResult> PutJsonAsync(
        string apiRoot,
        string authToken,
        string requestUri,
        string payload);
}