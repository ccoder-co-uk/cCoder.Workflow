// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Models;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

public sealed partial class WorkflowInstanceProcessingServiceTests
{
    [Theory]
    [InlineData(5)]
    [InlineData(30)]
    [InlineData(90)]
    public async Task ShouldUseEnvironmentConfiguredExecutionTimeoutForRecoveryAsync(int minutes)
    {
        // Given
        string prefix = "WorkflowTimeoutTest_" + Guid.NewGuid()
            .ToString(format: "N") + "_";

        string variable = prefix + "Workflow__QueueInstanceManagement__ExecutingTimeoutMinutes";

        Environment.SetEnvironmentVariable(variable: variable, value: minutes.ToString());

        try
        {
            IConfigurationRoot settings = new ConfigurationBuilder()
                .AddInMemoryCollection(initialData: new Dictionary<string, string> { ["Workflow:QueueInstanceManagement:ExecutingTimeoutMinutes"] = "30" })
                .AddEnvironmentVariables(prefix: prefix)
                .Build();

            settings.GetSection(key: "Workflow")
                .Bind(instance: configuration);

            DateTimeOffset observedCutoff = default;

            workflowInstanceManagementBrokerMock.Setup(expression: broker => broker.SelectExpiredInstances(cutoff: It.IsAny<DateTimeOffset>()))
                .Callback<DateTimeOffset>(action: value => observedCutoff = value)
                .Returns(value: []);

            workflowInstanceManagementBrokerMock.Setup(expression: broker => broker.FlushOldInstancesAsync(
                    cutoff: It.IsAny<DateTimeOffset>(), cancellationToken: It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: 0);

            DateTimeOffset earliest = DateTimeOffset.UtcNow.AddMinutes(minutes: -minutes);

            // When
            await processingService.RunInstanceMaintenanceAsync();

            // Then
            DateTimeOffset latest = DateTimeOffset.UtcNow.AddMinutes(minutes: -minutes);

            Assert.InRange(actual: observedCutoff, low: earliest, high: latest);

            Assert.Equal(expected: minutes, actual: configuration.QueueInstanceManagement.ExecutingTimeoutMinutes);

        }
        finally
        {
            Environment.SetEnvironmentVariable(variable: variable, value: null);

        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public async Task ShouldReportInvalidExecutionTimeoutWithoutMutatingInstancesAsync(double minutes)
    {
        // Given
        configuration.QueueInstanceManagement.ExecutingTimeoutMinutes = minutes;

        // When
        await processingService.RunInstanceMaintenanceAsync();

        // Then
        workflowInstanceManagementBrokerMock.VerifyNoOtherCalls();

        loggingBrokerMock.Verify(expression: broker => broker.LogError(exception: It.IsAny<Exception>(), message: It.IsAny<string>(), args: It.IsAny<object[]>()), times: Times.AtLeastOnce);

    }

    [Fact]
    public async Task ShouldPersistExpiredInstanceFailureThroughPeriodicMaintenanceAsync()
    {
        // Given
        FlowInstanceData instance = new()
        {
            Id = Guid.NewGuid(), FlowDefinitionId = Guid.NewGuid(), State = "Executing", Caller = "original",
            Start = DateTimeOffset.UtcNow.AddHours(hours: -2), ContextString = "{\"Flow\":{\"Data\":42}}"
        };

        workflowInstanceManagementBrokerMock.Setup(expression: broker => broker.SelectExpiredInstances(cutoff: It.IsAny<DateTimeOffset>()))
            .Returns(value: [instance]);

        workflowInstanceManagementBrokerMock.Setup(expression: broker => broker.CompareExchangeInstanceAsync(
                workflowInstanceExchange: It.Is<WorkflowInstanceExchange>(match: value => value.Expected == instance && value.Replacement.State == "Failed" && value.Replacement.Caller == "original"),
                cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: 1);

        workflowInstanceManagementBrokerMock.Setup(expression: broker => broker.FlushOldInstancesAsync(
                cutoff: It.IsAny<DateTimeOffset>(), cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: 0);

        // When
        await processingService.RunInstanceMaintenanceAsync();

        // Then
        workflowInstanceManagementBrokerMock.VerifyAll();

        loggingBrokerMock.Verify(expression: broker => broker.LogWarning(
            message: "Marked {Count} abandoned workflow executions Failed for administrative recovery.", args: It.Is<object[]>(match: value => (int)value[0] == 1)), times: Times.Once);

    }

    [Fact]
    public async Task ShouldPersistTerminalFailureThroughTrustedManagerAsync()
    {
        // Given
        Guid attempt = Guid.NewGuid();

        FlowInstanceData instance = new()
        {
            Id = Guid.NewGuid(), FlowDefinitionId = Guid.NewGuid(), State = "AwaitingExecution", Caller = "original",
            ContextString = "{\"ExecutionAttemptId\":\"" + attempt + "\"}"
        };

        workflowInstanceManagementBrokerMock.Setup(expression: broker => broker.SelectClaimedInstanceAsync(
                flowInstanceDataId: instance.Id, cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: instance);

        workflowInstanceManagementBrokerMock.Setup(expression: broker => broker.CompareExchangeInstanceAsync(
                workflowInstanceExchange: It.IsAny<WorkflowInstanceExchange>(), cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: 1);

        // When
        bool result = await processingService.RecordTerminalFailureAsync(instanceId: instance.Id, flowId: instance.FlowDefinitionId,
            attemptId: attempt, failureDescription: "HTTP 404");

        // Then
        Assert.True(condition: result);

        workflowInstanceManagementBrokerMock.VerifyAll();

    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldReportTrustedTerminalFailurePersistenceErrorsAsync(Exception exception, Type expectedType)
    {
        // Given
        workflowInstanceManagementBrokerMock.Setup(expression: broker => broker.SelectClaimedInstanceAsync(
                flowInstanceDataId: It.IsAny<Guid>(), cancellationToken: It.IsAny<CancellationToken>()))
            .Throws(exception: exception);

        // When
        Exception failure = await Record.ExceptionAsync(testCode: async () =>
            await processingService.RecordTerminalFailureAsync(instanceId: Guid.NewGuid(), flowId: Guid.NewGuid(),
                attemptId: Guid.NewGuid(), failureDescription: "failure"));

        // Then
        Assert.IsType(expectedType: expectedType, @object: failure);

    }

}