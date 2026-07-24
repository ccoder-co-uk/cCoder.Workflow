// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Text;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Activities.Support;
using cCoder.Workflow.Engine.Support;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace cCoder.Workflow.Engine.Services.Orchestrations;

public sealed class FlowExecutionOrchestrationService(
    ILogger<FlowExecutionOrchestrationService> logger)
    : IFlowExecutionOrchestrationService
{
    private HubConnection connection;

    public async Task ExecuteAsync(WorkflowRequest request)
    {
        ArgumentNullException.ThrowIfNull(argument:request);

        await ConnectToHubAsync(request:request);

        try
        {
            await LogAsync(level:WorkflowLogLevel.Info, message:"Request received by workflow, processing ...", instanceId:request.InstanceId);
            await LogAsync(level:WorkflowLogLevel.Debug, message:WorkflowJson.ToJson(request), instanceId:request.InstanceId);

            FlowInstance instance = new((level, message) => LogAsync(level:level, message:message, instanceId:request.InstanceId));
            FlowInstanceData result = await instance.ExecuteAsync(request:request);
            await SaveResultAsync(result:result, apiRoot:request.Api, authToken:request.AuthToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception:exception, message:"Workflow execution failed for instance {InstanceId}.", args:request.InstanceId);

            await LogAsync(
level:                WorkflowLogLevel.Fatal,
message:                $"Failed to process request, abandoning execution{Environment.NewLine}{exception.Message}{Environment.NewLine}{exception.StackTrace}",
instanceId:                request.InstanceId);
            throw;
        }
        finally
        {
            await LogAsync(level:WorkflowLogLevel.Info, message:"Done!", instanceId:request.InstanceId);
        }
    }

    private async Task LogAsync(WorkflowLogLevel level, string message, Guid instanceId)
    {
        if (message?.Length > 4000 && !message.Contains(value:"Failed to deserialise", comparisonType:StringComparison.OrdinalIgnoreCase))
            message = $"{message[..1900]} ... {message.Length - 1900} characters cut due to excessive length.";

        Console.WriteLine(value:$"{level}:: {message}");

        try
        {
            if (connection is not null)
                await connection.InvokeAsync(methodName:"ConsoleSend", arg1:level.ToString().ToLowerInvariant(), arg2:message, arg3:instanceId.ToString());
        }
        catch (Exception exception)
        {
            if (connection is not null)
                await connection.DisposeAsync();

            connection = null;
            await LogAsync(level:WorkflowLogLevel.Error, message:exception.Message, instanceId:instanceId);
            await LogAsync(level:level, message:message, instanceId:instanceId);
        }
        finally
        {
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    private async Task ConnectToHubAsync(WorkflowRequest request)
    {
        try
        {
            connection = new HubConnectionBuilder()
                .WithUrl(url:$"{request.Api}Hubs/Workflow", configureHttpConnection:options =>
                {
                    options.HttpMessageHandlerFactory = handler =>
                    {
                        if (handler is HttpClientHandler clientHandler)
                            clientHandler.ServerCertificateCustomValidationCallback += CertChainValidator.ValidateCertChain;

                        return handler;
                    };
                })
                .Build();

            connection.On<Exception>(methodName:"error", handler:exception => Console.WriteLine($"{exception.Message}{Environment.NewLine}{exception.StackTrace}"));

            await connection.StartAsync();
            await LogAsync(level:WorkflowLogLevel.Info, message:$"Workflow instance {request.InstanceId} connected.", instanceId:request.InstanceId);
        }
        catch (Exception exception)
        {
            if (connection is not null)
                await connection.DisposeAsync();

            connection = null;
            await LogAsync(level:WorkflowLogLevel.Warning, message:$"Workflow hub connection could not be established: {exception.Message}", instanceId:request.InstanceId);
        }
    }

    private static async Task SaveResultAsync(FlowInstanceData result, string apiRoot, string authToken)
    {
        using HttpClient api = CreateApiClient(apiRoot:apiRoot);
        api.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
        string payload = JsonConvert.SerializeObject(
value:            new
            {
                result.Id,
                result.FlowDefinitionId,
                result.Name,
                result.State,
                result.ReportingComponentName,
                result.Caller,
                ContextString = result.ContextString,
                result.Start,
                result.End
            },
formatting:            Formatting.None);

        HttpResponseMessage response = await api.PutAsync(
requestUri:            $"Workflow/FlowInstanceData({result.Id})",
content:            new StringContent(payload, Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();

            throw new HttpRequestException(
                $"Workflow result save failed with status {(int)response.StatusCode} ({response.StatusCode})."
                + $"{Environment.NewLine}Payload:{Environment.NewLine}{payload}"
                + $"{Environment.NewLine}Response:{Environment.NewLine}{responseBody}");
        }

        response.EnsureSuccessStatusCode();
    }

    private static HttpClient CreateApiClient(string apiRoot) => new(new HttpClientHandler
    {
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
        ServerCertificateCustomValidationCallback = CertChainValidator.ValidateCertChain
    })
    {
        BaseAddress = new Uri(apiRoot)
    };
}