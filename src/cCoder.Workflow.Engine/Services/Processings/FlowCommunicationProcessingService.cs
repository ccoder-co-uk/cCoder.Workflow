// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Activities.Support;
using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Extensions;
using Microsoft.Extensions.Logging;

namespace cCoder.Workflow.Engine.Services.Processings;

internal sealed partial class FlowCommunicationProcessingService(
    cCoder.Workflow.Engine.Brokers.Loggings.ILoggingBroker logger,
    IWorkflowHubConnectionBroker workflowHubConnectionBroker)
    : IFlowCommunicationProcessingService
{
    private bool isConnected;

    public ValueTask ConnectWorkflowRequestAsync(
        WorkflowRequest workflowRequest) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [workflowRequest]);

            try
            {
                await workflowHubConnectionBroker.ConnectAsync(
                    url: $"{workflowRequest.Api}Hubs/Workflow");

                isConnected = true;

                await ExecuteLogWorkflowRequestAsync(
                    workflowRequest: workflowRequest,
                    level: WorkflowLogLevel.Info,
                    message:
                        $"Workflow instance "
                        + $"{workflowRequest.InstanceId} connected.");
            }
            catch (Exception exception)
            {
                await workflowHubConnectionBroker.DisconnectAsync();
                isConnected = false;

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
            if (isConnected)
            {
                await workflowHubConnectionBroker.SendAsync(
                    level: level.ToString()
                        .ToLowerInvariant(),
                    message: message,
                    instanceId: workflowRequest.InstanceId.ToString());
            }
        }
        catch (Exception exception)
        {
            await workflowHubConnectionBroker.DisconnectAsync();
            isConnected = false;

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