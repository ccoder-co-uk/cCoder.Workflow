// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Models.Exceptions;
using cCoder.Workflow.Engine.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class FlowResultServiceTests
{
    [Fact]
    public void ShouldDeserializeFlowResult()
    {
        // Given
        const string value = "{\"name\":\"workflow\"}";
        var expected = new SerializationModel { Name = "workflow" };
        var jsonBrokerMock = new Mock<IJsonBroker>(MockBehavior.Strict);

        jsonBrokerMock
            .Setup(expression: broker =>
                broker.Deserialize<SerializationModel>(value: value))
            .Returns(value: expected);

        var service = new FlowResultService(
            workflowHttpClientBroker: workflowHttpClientBrokerMock.Object,
            jsonBroker: jsonBrokerMock.Object);

        // When
        SerializationModel actual =
            service.Deserialize<SerializationModel>(value: value);

        // Then
        actual
            .Should()
            .BeSameAs(expected: expected);

        jsonBrokerMock.VerifyAll();
    }

    [Fact]
    public void ShouldSerializeFlowResult()
    {
        // Given
        const string expected = "{\"name\":\"workflow\"}";
        var value = new SerializationModel { Name = "workflow" };
        var jsonBrokerMock = new Mock<IJsonBroker>(MockBehavior.Strict);

        jsonBrokerMock
            .Setup(expression: broker => broker.Serialize(value: value))
            .Returns(value: expected);

        var service = new FlowResultService(
            workflowHttpClientBroker: workflowHttpClientBrokerMock.Object,
            jsonBroker: jsonBrokerMock.Object);

        // When
        string actual = service.Serialize(value: value);

        // Then
        actual
            .Should()
            .Be(expected: expected);

        jsonBrokerMock.VerifyAll();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ShouldRejectInvalidFlowResultDeserialization(
        string value)
    {
        // Given
        var jsonBrokerMock = new Mock<IJsonBroker>(MockBehavior.Strict);

        var service = new FlowResultService(
            workflowHttpClientBroker: workflowHttpClientBrokerMock.Object,
            jsonBroker: jsonBrokerMock.Object);

        // When
        Action action = () => service.Deserialize<SerializationModel>(
            value: value);

        // Then
        action
            .Should()
            .Throw<WorkflowEngineValidationException>();

        jsonBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldRejectInvalidFlowResultSerialization()
    {
        // Given
        var jsonBrokerMock = new Mock<IJsonBroker>(MockBehavior.Strict);

        var service = new FlowResultService(
            workflowHttpClientBroker: workflowHttpClientBrokerMock.Object,
            jsonBroker: jsonBrokerMock.Object);

        // When
        Action action = () => service.Serialize(value: null);

        // Then
        action
            .Should()
            .Throw<WorkflowEngineValidationException>();

        jsonBrokerMock.VerifyNoOtherCalls();
    }

    [Theory]
    [MemberData(
        nameof(WorkflowRequestOrchestrationServiceTests.ExceptionMappings),
        MemberType = typeof(WorkflowRequestOrchestrationServiceTests))]
    public void ShouldMapFlowResultSerializationFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        var value = new SerializationModel { Name = "workflow" };
        var jsonBrokerMock = new Mock<IJsonBroker>(MockBehavior.Strict);

        jsonBrokerMock
            .Setup(expression: broker => broker.Serialize(value: value))
            .Throws(exception: exception);

        var service = new FlowResultService(
            workflowHttpClientBroker: workflowHttpClientBrokerMock.Object,
            jsonBroker: jsonBrokerMock.Object);

        // When
        Action action = () => service.Serialize(value: value);

        // Then
        action
            .Should()
            .Throw<Exception>()
            .Which
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    private sealed class SerializationModel
    {
        public string Name { get; init; }
    }
}