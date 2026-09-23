// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Azure.Functions.Worker.Http;

namespace Workflow.Services.Foundations.WorkflowHttpResponses;

internal interface IWorkflowHttpResponseService
{
    Task<HttpResponseData> CreateHttpResponseDataAsync(
        HttpRequestData request,
        string content);
}