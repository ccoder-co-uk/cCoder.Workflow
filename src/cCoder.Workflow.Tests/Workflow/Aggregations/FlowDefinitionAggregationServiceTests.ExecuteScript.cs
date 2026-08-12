// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Net.Sockets;
using System.Text;
using cCoder.Workflow.Dependencies.ServiceProviders;
using cCoder.Workflow.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests.Workflow.Aggregations;

public partial class FlowDefinitionAggregationServiceTests
{
    [Fact]
    public async Task ShouldExecuteScriptThroughWorkflowApiAsync()
    {
        // Given
        using TcpListener listener = new(IPAddress.Loopback, port: 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;

        WorkflowConfiguration configuration = new()
        {
            ServiceUrl = $"http://127.0.0.1:{port}/"
        };

        serviceProviderBrokerMock
            .Setup(expression: broker => broker
                .GetOperationService<WorkflowConfiguration>(
                    operation: FlowDefinitionOperation.Configuration))
            .Returns(value: configuration);

        Task responseTask = Task.Run(async () =>
        {
            using TcpClient client = await listener.AcceptTcpClientAsync();
            using NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[4096];
            _ = await stream.ReadAsync(buffer);
            byte[] response = Encoding.ASCII.GetBytes(
                "HTTP/1.1 200 OK\r\nContent-Length: 2\r\nConnection: close\r\n\r\nok");

            await stream.WriteAsync(response);
        });

        // When
        string result = await service.ExecuteScriptAsync(script: "return 1;");
        await responseTask;

        // Then
        result.Should().Be(expected: "ok");
        serviceProviderBrokerMock.VerifyAll();
    }
}