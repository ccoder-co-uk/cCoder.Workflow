// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class FlowDefinitionProcessingServiceTests
{
    [Fact]
    public void ShouldAuthorizeFlowDefinitionExecution()
    {
        // Given
        authorizationBrokerMock
            .Setup(expression: broker => broker.Authorize(
                userId: "user",
                appId: 7,
                privilege: "flowdefinition_execute"));

        // When
        bool result = flowDefinitionProcessingService
            .AuthorizeFlowDefinitionExecution(userId: "user", appId: 7);

        // Then
        result.Should().BeTrue();
        authorizationBrokerMock.VerifyAll();
    }

    [Fact]
    public void ShouldParseFlowDefinition()
    {
        // Given
        Flow expected = new();

        jsonBrokerMock
            .Setup(expression: broker => broker.ParseJson<Flow>(json: "{}"))
            .Returns(value: expected);

        // When
        object actual = flowDefinitionProcessingService
            .ParseFlowDefinition(definitionJson: "{}");

        // Then
        actual.Should().BeSameAs(expected: expected);
        jsonBrokerMock.VerifyAll();
    }

    [Fact]
    public void ShouldParseFlowDefinitionData()
    {
        // Given
        object expected = new();

        jsonBrokerMock
            .Setup(expression: broker => broker.ParseJson(json: "{}"))
            .Returns(value: expected);

        // When
        object actual = flowDefinitionProcessingService
            .ParseFlowDefinitionData(args: "{}");

        // Then
        actual.Should().BeSameAs(expected: expected);
        jsonBrokerMock.VerifyAll();
    }

    [Fact]
    public void ShouldSerializeFlowDefinitionContext()
    {
        // Given
        object context = new();

        jsonBrokerMock
            .Setup(expression: broker => broker.Serialize(value: context))
            .Returns(value: "serialized");

        // When
        string actual = flowDefinitionProcessingService
            .SerializeFlowDefinitionContext(context: context);

        // Then
        actual.Should().Be(expected: "serialized");
        jsonBrokerMock.VerifyAll();
    }
}