// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Security.Exposures;
using cCoder.Security.Models.Entities;
using cCoder.Workflow.Activities.Models;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

public sealed partial class WorkflowInstanceProcessingServiceTests
{
    [Fact]
    public async Task ShouldIgnoreMissingClaimedWorkflowInstanceAsync()
    {
        // Given
        Guid instanceId = Guid.NewGuid();

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.UpdateQueuedInstanceClaimAsync(
                flowInstanceDataId: instanceId,
                cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: 1);

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.SelectClaimedInstanceAsync(
                flowInstanceDataId: instanceId,
                cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: (FlowInstanceData)null);

        // When
        await processingService.ExecuteWaitingQueuedInstanceByIdAsync(
            flowInstanceDataId: instanceId);

        // Then
        workflowInstanceManagementBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldMarkClaimedWorkflowInstanceFailedWhenTokenIssueFailsAsync()
    {
        // Given
        FlowInstanceData instance = CreateQueuedFlowInstanceData();
        Exception exception = new(message: "Token issue failed");

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.UpdateQueuedInstanceClaimAsync(
                flowInstanceDataId: instance.Id,
                cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: 1);

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.SelectClaimedInstanceAsync(
                flowInstanceDataId: instance.Id,
                cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: instance);

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.MarkInstanceFailedAsync(
                flowInstanceDataId: instance.Id,
                failedAt: It.IsAny<DateTimeOffset>(),
                cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: 1);

        serviceProviderMock
            .Setup(expression: provider => provider.GetService(
                serviceType: typeof(ITokenManager)))
            .Throws(exception: exception);

        loggingBrokerMock
            .Setup(expression: broker => broker.LogError(
                exception: exception,
                message: "Flow instance {InstanceId} execution failed.",
                args: instance.Id));

        // When
        await processingService.ExecuteWaitingQueuedInstanceByIdAsync(
            flowInstanceDataId: instance.Id);

        // Then
        workflowInstanceManagementBrokerMock.VerifyAll();
        serviceProviderMock.VerifyAll();
        loggingBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldMarkClaimedWorkflowInstanceFailedWhenWorkflowApiFailsAsync()
    {
        // Given
        FlowInstanceData instance = CreateQueuedFlowInstanceData();
        Mock<ITokenManager> tokenManagerMock = new();
        configuration.ServiceUrl = "http://127.0.0.1:1/";

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.UpdateQueuedInstanceClaimAsync(
                flowInstanceDataId: instance.Id,
                cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: 1);

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.SelectClaimedInstanceAsync(
                flowInstanceDataId: instance.Id,
                cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: instance);

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.MarkInstanceFailedAsync(
                flowInstanceDataId: instance.Id,
                failedAt: It.IsAny<DateTimeOffset>(),
                cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: 1);

        serviceProviderMock
            .Setup(expression: provider => provider.GetService(
                serviceType: typeof(ITokenManager)))
            .Returns(value: tokenManagerMock.Object);

        tokenManagerMock
            .Setup(expression: manager => manager.IssueTokenAsync(
                userId: instance.Caller,
                tokenUse: TokenUse.WorkflowExecution))
            .ReturnsAsync(value: new Token { Id = "token" });

        // When
        await processingService.ExecuteWaitingQueuedInstanceByIdAsync(
            flowInstanceDataId: instance.Id);

        // Then
        workflowInstanceManagementBrokerMock.VerifyAll();
        serviceProviderMock.VerifyAll();
        tokenManagerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldMarkClaimedWorkflowInstanceFailedForUnsuccessfulResponseAsync()
    {
        // Given
        FlowInstanceData instance = CreateQueuedFlowInstanceData();
        Mock<ITokenManager> tokenManagerMock = new();
        using TcpListener listener = new(IPAddress.Loopback, port: 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        configuration.ServiceUrl = $"http://127.0.0.1:{port}/";

        Task responseTask = Task.Run(async () =>
        {
            using TcpClient client = await listener.AcceptTcpClientAsync();
            using NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[4096];
            _ = await stream.ReadAsync(buffer);
            byte[] response = Encoding.ASCII.GetBytes(
                "HTTP/1.1 500 Internal Server Error\r\n"
                + "Content-Length: 6\r\nConnection: close\r\n\r\nfailed");

            await stream.WriteAsync(response);
        });

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.UpdateQueuedInstanceClaimAsync(
                flowInstanceDataId: instance.Id,
                cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: 1);

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.SelectClaimedInstanceAsync(
                flowInstanceDataId: instance.Id,
                cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: instance);

        workflowInstanceManagementBrokerMock
            .Setup(expression: broker => broker.MarkInstanceFailedAsync(
                flowInstanceDataId: instance.Id,
                failedAt: It.IsAny<DateTimeOffset>(),
                cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: 1);

        serviceProviderMock
            .Setup(expression: provider => provider.GetService(
                serviceType: typeof(ITokenManager)))
            .Returns(value: tokenManagerMock.Object);

        tokenManagerMock
            .Setup(expression: manager => manager.IssueTokenAsync(
                userId: instance.Caller,
                tokenUse: TokenUse.WorkflowExecution))
            .ReturnsAsync(value: new Token { Id = "token" });

        // When
        await processingService.ExecuteWaitingQueuedInstanceByIdAsync(
            flowInstanceDataId: instance.Id);

        await responseTask;

        // Then
        workflowInstanceManagementBrokerMock.VerifyAll();
        serviceProviderMock.VerifyAll();
        tokenManagerMock.VerifyAll();
    }
}