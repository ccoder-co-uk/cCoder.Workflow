// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Exposures;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text;
using Workflow.Dependencies;
using Workflow.Brokers.Http;
using Workflow.Brokers.Loggings;

namespace Workflow.Services.Processings.WorkflowFunctions;

internal sealed partial class WorkflowFunctionsProcessingService(
    IFlowRunner flowRunner,
    IWorkflowScriptExecutionService scriptExecutionService,
    IJsonBroker jsonBroker,
    IHttpResponseBroker httpResponseBroker,
    ILoggingBroker loggingBroker)
        : IWorkflowFunctionsProcessingService
{
    public Task<HttpResponseData> ProcessExecuteAsync(HttpRequestData request) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [request]);

            string json = await ReadBodyAsync(request: request);

            WorkflowRequest workflowRequest =
                jsonBroker.Deserialize<WorkflowRequest>(
                    value: json)
                ?? throw new InvalidOperationException(
                    message: "Workflow request payload could not be deserialized.");

            await flowRunner.RunAsync(workflowRequest: workflowRequest);

            return await CreateHttpResponseDataAsync(
                request: request,
                content: "OK");
        });

    public Task<HttpResponseData> ProcessExecuteScriptAsync(
        HttpRequestData request,
        bool useDetails) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [request, useDetails]);

            string payload = await ReadBodyAsync(request: request);

            string result = await scriptExecutionService.ExecuteAsync(
                payload: payload,
                useDetails: useDetails);

            return await CreateHttpResponseDataAsync(
                request: request,
                content: result);
        });

    public Task<HttpResponseData> ProcessHealthAsync(HttpRequestData request) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [request]);

            return CreateHttpResponseDataAsync(
                request: request,
                content: "OK");
        });

    public Task ProcessServiceBusMessageAsync(string message) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [message]);

            loggingBroker.LogInformation(
                message:
                    "Service Bus workflow trigger is scaffolded but disabled.");

            return Task.CompletedTask;
        });

    private async Task<HttpResponseData> CreateHttpResponseDataAsync(
        HttpRequestData request,
        string content)
    {
        HttpResponseData response = httpResponseBroker.CreateResponse(
            request: request,
            statusCode: HttpStatusCode.OK);

        await httpResponseBroker.WriteStringAsync(
            response: response,
            value: content);

        return response;
    }

    private static async ValueTask<string> ReadBodyAsync(
        HttpRequestData request)
    {
        using WorkflowFunctionStreamDependency content = new();
        await request.Body.CopyToAsync(destination: content);
        return Encoding.UTF8.GetString(bytes: content.ToArray());
    }
}