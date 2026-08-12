// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Models.Exceptions;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class WorkflowScriptExecutionProcessingServiceTests
{
    [Fact]
    public async Task ShouldExecuteWorkflowScriptAsync()
    {
        // Given
        const string payload = "return value";
        var result = new { Value = 7 };

        scriptBrokerMock
            .Setup(expression: broker => broker.Run<object>(
                code: payload,
                imports: It.IsAny<string[]>(),
                args: null,
                log: It.IsAny<Action<WorkflowLogLevel, string>>()))
            .ReturnsAsync(value: result);

        var service = CreateService();

        // When
        string actual = await service.ExecuteWorkflowScriptAsync(
            payload: payload,
            useDetails: false);

        // Then
        actual
            .Should()
            .Contain(expected: "\"Value\":7");

        scriptBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldExecuteWorkflowScriptDetailsAsync()
    {
        // Given
        const string script = "return value";
        const string expected = "result";
        JObject model = JObject.FromObject(o: new { Value = 7 });

        string payload = JsonConvert.SerializeObject(
            value: new ExecutionDetails
            {
                Script = script,
                Model = model
            });

        scriptBrokerMock
            .Setup(expression: broker => broker.Run<string>(
                code: script,
                imports: It.IsAny<string[]>(),
                args: It.Is<object>(match: argument =>
                    JToken.DeepEquals(
                        t1: argument as JToken,
                        t2: model)),
                log: It.IsAny<Action<WorkflowLogLevel, string>>()))
            .ReturnsAsync(value: expected);

        var service = CreateService();

        // When
        string actual = await service.ExecuteWorkflowScriptAsync(
            payload: payload,
            useDetails: true);

        // Then
        actual
            .Should()
            .Be(expected: expected);

        scriptBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldRouteWorkflowScriptLogsByLevelAsync()
    {
        // Given
        const string payload = "return value";
        const string message = "message";
        Action<WorkflowLogLevel, string> log = null;

        scriptBrokerMock
            .Setup(expression: broker => broker.Run<object>(
                code: payload,
                imports: It.IsAny<string[]>(),
                args: null,
                log: It.IsAny<Action<WorkflowLogLevel, string>>()))
            .Callback<string, string[], object,
                Action<WorkflowLogLevel, string>>(
                    action: (_, _, _, callback) => log = callback)
            .ReturnsAsync(value: new object());

        var service = CreateService();

        await service.ExecuteWorkflowScriptAsync(
            payload: payload,
            useDetails: false);

        // When
        log.Invoke(arg1: WorkflowLogLevel.Debug, arg2: message);
        log.Invoke(arg1: WorkflowLogLevel.Info, arg2: message);
        log.Invoke(arg1: WorkflowLogLevel.Warning, arg2: message);
        log.Invoke(arg1: WorkflowLogLevel.Error, arg2: message);
        log.Invoke(arg1: WorkflowLogLevel.Fatal, arg2: message);

        // Then
        loggingBrokerMock.Verify(
            expression: broker => broker.LogDebug(
                message: "{Message}",
                args: It.Is<object[]>(match: arguments =>
                    arguments.Single() as string == message)),
            times: Times.Once());

        loggingBrokerMock.Verify(
            expression: broker => broker.LogInformation(
                message: "{Message}",
                args: It.Is<object[]>(match: arguments =>
                    arguments.Single() as string == message)),
            times: Times.Once());

        loggingBrokerMock.Verify(
            expression: broker => broker.LogWarning(
                message: "{Message}",
                args: It.Is<object[]>(match: arguments =>
                    arguments.Single() as string == message)),
            times: Times.Once());

        loggingBrokerMock.Verify(
            expression: broker => broker.LogError(
                message: "{Message}",
                args: It.Is<object[]>(match: arguments =>
                    arguments.Single() as string == message)),
            times: Times.Exactly(callCount: 2));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ShouldRejectInvalidWorkflowScriptPayloadAsync(
        string payload)
    {
        // Given
        var service = CreateService();

        // When
        Func<Task> action = async () => await service
            .ExecuteWorkflowScriptAsync(payload: payload, useDetails: false);

        // Then
        await action
            .Should()
            .ThrowAsync<WorkflowEngineValidationException>();

        scriptBrokerMock.VerifyNoOtherCalls();
    }
}