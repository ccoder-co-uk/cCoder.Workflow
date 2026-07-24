// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Activities.Support;
using cCoder.Workflow.Engine.Support;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace cCoder.Workflow.Engine.Services.Processings;

internal sealed partial class FlowCommunicationProcessingService(
    ILogger<FlowCommunicationProcessingService> logger)
    : IFlowCommunicationProcessingService
{
    private HubConnection connection;

    public ValueTask ConnectWorkflowRequestAsync(
        WorkflowRequest workflowRequest) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [workflowRequest]);

            try
            {
                connection = new HubConnectionBuilder()
                    .WithUrl(
                        url: $"{workflowRequest.Api}Hubs/Workflow",
                        configureHttpConnection: options =>
                        {
                            options.HttpMessageHandlerFactory =
                                handler =>
                                {
                                    if (handler is HttpClientHandler
                                        clientHandler)
                                    {
                                        clientHandler
                                            .ServerCertificateCustomValidationCallback +=
                                            CertChainValidator
                                                .ValidateCertChain;
                                    }

                                    return handler;
                                };
                        })
                    .Build();

                connection.On<Exception>(
                    methodName: "error",
                    handler: exception => Console.WriteLine(
                        value:
                            $"{exception.Message}"
                            + $"{Environment.NewLine}"
                            + exception.StackTrace));

                await connection.StartAsync();

                await ExecuteLogWorkflowRequestAsync(
                    workflowRequest: workflowRequest,
                    level: WorkflowLogLevel.Info,
                    message:
                        $"Workflow instance "
                        + $"{workflowRequest.InstanceId} connected.");
            }
            catch (Exception exception)
            {
                if (connection is not null)
                {
                    await connection.DisposeAsync();
                }

                connection = null;

                await ExecuteLogWorkflowRequestAsync(
                    workflowRequest: workflowRequest,
                    level: WorkflowLogLevel.Warning,
                    message:
                        "Workflow hub connection could not be "
                        + $"established: {exception.Message}");
            }
        });

    public ValueTask LogWorkflowRequestAsync(
        WorkflowRequest workflowRequest,
        WorkflowLogLevel level,
        string message) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(
                inputs:
                [
                    workflowRequest,
                    level,
                    message
                ]);

            await ExecuteLogWorkflowRequestAsync(
                workflowRequest: workflowRequest,
                level: level,
                message: message);
        });

    private async ValueTask ExecuteLogWorkflowRequestAsync(
        WorkflowRequest workflowRequest,
        WorkflowLogLevel level,
        string message)
    {
        if (message?.Length > 4000
                && !message.Contains(
                    value: "Failed to deserialise",
                    comparisonType:
                        StringComparison.OrdinalIgnoreCase))
        {
            message =
                $"{message[..1900]} ... "
                + $"{message.Length - 1900} characters cut "
                + "due to excessive length.";
        }

        Console.WriteLine(value: $"{level}:: {message}");

        if (level is WorkflowLogLevel.Error
            or WorkflowLogLevel.Fatal)
        {
            logger.LogError(
                message: "{Message}",
                args: message);
        }

        try
        {
            if (connection is not null)
            {
                await connection.InvokeAsync(
                    methodName: "ConsoleSend",
                    arg1: level.ToString()
                        .ToLowerInvariant(),
                    arg2: message,
                    arg3:
                        workflowRequest.InstanceId.ToString());
            }
        }
        catch (Exception exception)
        {
            if (connection is not null)
            {
                await connection.DisposeAsync();
            }

            connection = null;

            await ExecuteLogWorkflowRequestAsync(
                workflowRequest: workflowRequest,
                level: WorkflowLogLevel.Error,
                message: exception.Message);

            await ExecuteLogWorkflowRequestAsync(
                workflowRequest: workflowRequest,
                level: level,
                message: message);
        }
        finally
        {
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}