// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities;
using cCoder.Workflow.Activities.Activities;
using cCoder.Workflow.Activities.Activities.Api;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Dependencies;
using cCoder.Workflow.Engine.Extensions;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Models.Exceptions;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class FlowInstanceProcessingServiceTests
{
    [Fact]
    public async Task ShouldExecuteFlowExecutionAsync()
    {
        // Given
        FlowExecution execution = CreateFlowExecution();

        Start start = new() { Ref = "start" };
        InfoActivity info = new() { Ref = "info", Message = "done" };
        ApiGet<string> apiGet = new() { Ref = "api", Result = "ignored" };

        Flow flow = new()
        {
            Name = "Flow",
            Activities = [start, info, apiGet],
            Links =
            [
                new Link
                {
                    Source = start.Ref,
                    Destination = info.Ref,
                    Expression = "destination.Message = source.Ref"
                }
            ]
        };

        WorkflowContext workflowContext = new()
        {
            Flow = flow,
            ExecutionState = "Complete"
        };

        FlowInstanceData instanceData = new()
        {
            Id = execution.Request.InstanceId,
            FlowDefinitionId = execution.Request.FlowId,
            Name = "Instance",
            Caller = "caller",
            ContextString = JsonConvert.SerializeObject(
                value: workflowContext,
                settings: ObjectExtensions.GetJsonSettings()),

            FlowDefinition = new()
            {
                Id = execution.Request.FlowId,
                AppId = 7
            }
        };

        string rawInstance = JsonConvert.SerializeObject(
            value: instanceData,
            settings: ObjectExtensions.GetJsonSettings());

        workflowHttpClientBrokerMock
            .Setup(expression: broker => broker.GetStringAsync(
                apiRoot: execution.Request.Api,
                authToken: execution.Request.AuthToken,
                requestUri: It.IsAny<string>()))
            .Returns(value: ValueTask.FromResult(result: rawInstance));

        workflowContextBrokerMock
            .Setup(expression: broker => broker.CreateWorkflowExecutionContext(
                flowExecution: execution))
            .Returns(valueFunction: () => new WorkflowExecutionContext(
                flowExecution: execution));

        workflowContextBrokerMock
            .Setup(expression: broker => broker
                .ExecuteWorkflowExecutionContextAsync(
                    workflowExecutionContext: It.IsAny<WorkflowExecutionContext>(),
                    apiRoot: execution.Request.Api,
                    authToken: execution.Request.AuthToken))
            .Callback<WorkflowExecutionContext, string, string>(
                action: (context, _, _) =>
                    context.ExecutionState = workflowContext.ExecutionState)
            .Returns(value: Task.CompletedTask);

        var service = CreateService();

        // When
        FlowExecution actual = await service.ExecuteFlowExecutionAsync(
            flowExecution: execution);

        // Then
        actual
            .Should()
            .BeSameAs(expected: execution);

        actual.AppId
            .Should()
            .Be(expected: instanceData.FlowDefinition.AppId);

        actual.Result.Id
            .Should()
            .Be(expected: instanceData.Id);

        actual.Result.State
            .Should()
            .Be(expected: workflowContext.ExecutionState);

        actual.Script
            .Should()
            .BeSameAs(expected: scriptBrokerMock.Object);

        Activity actualStart = actual.Flow.Activities.Single(
            predicate: activity => activity.Ref == start.Ref);

        Activity actualInfo = actual.Flow.Activities.Single(
            predicate: activity => activity.Ref == info.Ref);

        actualStart.Next
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeSameAs(expected: actualInfo);

        actualInfo.Previous
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeSameAs(expected: actualStart);

        actualInfo.AssignCode
            .Should()
            .Contain(expected: "flow.GetActivity");

        workflowHttpClientBrokerMock.VerifyAll();
        workflowContextBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldRejectInvalidFlowExecutionAsync()
    {
        // Given
        var service = CreateService();

        // When
        Func<Task> action = async () => await service
            .ExecuteFlowExecutionAsync(flowExecution: null);

        // Then
        await action
            .Should()
            .ThrowAsync<WorkflowEngineValidationException>();

        workflowHttpClientBrokerMock.VerifyNoOtherCalls();
        workflowContextBrokerMock.VerifyNoOtherCalls();
        scriptBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldLogInvalidFlowInstanceResponseAsync()
    {
        // Given
        FlowExecution execution = CreateFlowExecution();
        List<string> messages = [];

        execution.Log = (level, message) =>
        {
            messages.Add(item: message);
            return Task.CompletedTask;
        };

        workflowHttpClientBrokerMock
            .Setup(expression: broker => broker.GetStringAsync(
                apiRoot: execution.Request.Api,
                authToken: execution.Request.AuthToken,
                requestUri: It.IsAny<string>()))
            .Returns(value: ValueTask.FromResult(result: "invalid"));

        var service = CreateService();

        // When
        Func<Task> action = async () => await service
            .ExecuteFlowExecutionAsync(flowExecution: execution);

        // Then
        await action
            .Should()
            .ThrowAsync<WorkflowEngineServiceException>();

        messages
            .Should()
            .ContainSingle(predicate: message => message.Contains(
                value: "Failed to deserialize flow instance"));
    }

    [Fact]
    public async Task ShouldLogInvalidWorkflowContextAsync()
    {
        // Given
        FlowExecution execution = CreateFlowExecution();
        List<string> messages = [];

        execution.Log = (level, message) =>
        {
            messages.Add(item: message);
            return Task.CompletedTask;
        };

        string rawInstance = SerializeFlowInstanceData(
            execution: execution,
            contextString: "invalid");

        workflowHttpClientBrokerMock
            .Setup(expression: broker => broker.GetStringAsync(
                apiRoot: execution.Request.Api,
                authToken: execution.Request.AuthToken,
                requestUri: It.IsAny<string>()))
            .Returns(value: ValueTask.FromResult(result: rawInstance));

        var service = CreateService();

        // When
        Func<Task> action = async () => await service
            .ExecuteFlowExecutionAsync(flowExecution: execution);

        // Then
        await action
            .Should()
            .ThrowAsync<WorkflowEngineServiceException>();

        messages
            .Should()
            .ContainSingle(predicate: message => message.Contains(
                value: "Failed to deserialize flow context"));
    }

    [Fact]
    public async Task ShouldLogMalformedFlowLinksAsync()
    {
        // Given
        FlowExecution execution = CreateFlowExecution();
        List<string> messages = [];

        execution.Log = (level, message) =>
        {
            messages.Add(item: message);

            if (message.Contains(value: "previous activity selection"))
            {
                Activity activity = execution.Flow.Activities.Single();
                activity.Previous = [activity];
                execution.Flow.Links = Array.Empty<Link>();
            }

            return Task.CompletedTask;
        };

        WorkflowContext context = new()
        {
            Flow = new Flow
            {
                Name = "Malformed",
                Activities =
                [
                    new InfoActivity
                    {
                        Ref = "info",
                        Message = "message"
                    }
                ],

                Links = null
            }
        };

        string rawInstance = SerializeFlowInstanceData(
            execution: execution,
            contextString: JsonConvert.SerializeObject(
                value: context,
                settings: ObjectExtensions.GetJsonSettings()));

        workflowHttpClientBrokerMock
            .Setup(expression: broker => broker.GetStringAsync(
                apiRoot: execution.Request.Api,
                authToken: execution.Request.AuthToken,
                requestUri: It.IsAny<string>()))
            .Returns(value: ValueTask.FromResult(result: rawInstance));

        workflowContextBrokerMock
            .Setup(expression: broker => broker.CreateWorkflowExecutionContext(
                flowExecution: execution))
            .Throws(exception: new Exception("stop after stitching"));

        var service = CreateService();

        // When
        Func<Task> action = async () => await service
            .ExecuteFlowExecutionAsync(flowExecution: execution);

        // Then
        await action
            .Should()
            .ThrowAsync<WorkflowEngineServiceException>();

        messages
            .Should()
            .HaveCount(expected: 2);

        messages
            .Should()
            .Contain(predicate: message => message.Contains(
                value: "previous activity selection"));

        messages
            .Should()
            .Contain(predicate: message => message.Contains(
                value: "one or more links"));

    }
}