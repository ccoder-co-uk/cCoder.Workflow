// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Support;
using Microsoft.AspNetCore.SignalR.Client;

namespace cCoder.Workflow.Engine.Dependencies;

internal sealed class WorkflowHubConnectionDependency : IAsyncDisposable
{
    private readonly HubConnection connection;

    internal WorkflowHubConnectionDependency(string url)
    {
        connection = new HubConnectionBuilder()
            .WithUrl(
                url: url,
                configureHttpConnection: options =>
                {
                    options.HttpMessageHandlerFactory = handler =>
                    {
                        if (handler is HttpClientHandler clientHandler)
                        {
                            clientHandler
                                .ServerCertificateCustomValidationCallback +=
                                CertChainValidator.ValidateCertChain;
                        }

                        return handler;
                    };
                })
            .Build();

        connection.On<Exception>(
            methodName: "error",
            handler: exception => Console.WriteLine(
                value:
                    $"{exception.Message}{Environment.NewLine}"
                    + exception.StackTrace));
    }

    internal Task ConnectAsync() =>
        connection.StartAsync();

    internal Task SendAsync(
        string level,
        string message,
        string instanceId) =>
        connection.InvokeAsync(
            methodName: "ConsoleSend",
            arg1: level,
            arg2: message,
            arg3: instanceId);

    public ValueTask DisposeAsync() =>
        connection.DisposeAsync();
}