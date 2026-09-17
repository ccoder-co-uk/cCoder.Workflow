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
        flowDefinitionServiceMock
            .Setup(expression: service => service.AuthorizeExecution(
                userId: "user",
                appId: 7))
            .Returns(value: true);

        // When
        bool result = flowDefinitionProcessingService
            .AuthorizeFlowDefinitionExecution(userId: "user", appId: 7);

        // Then
        result.Should()
            .BeTrue();

        flowDefinitionServiceMock.VerifyAll();
    }

    [Fact]
    public void ShouldParseFlowDefinition()
    {
        // Given
        Flow expected = new();

        flowDefinitionServiceMock
            .Setup(expression: service => service.ParseDefinition(
                definitionJson: "{}"))
            .Returns(value: expected);

        // When
        object actual = flowDefinitionProcessingService
            .ParseFlowDefinition(definitionJson: "{}");

        // Then
        actual.Should()
            .BeSameAs(expected: expected);

        flowDefinitionServiceMock.VerifyAll();
    }

    [Fact]
    public void ShouldParseFlowDefinitionData()
    {
        // Given
        object expected = new();

        flowDefinitionServiceMock
            .Setup(expression: service => service.ParseData(args: "{}"))
            .Returns(value: expected);

        // When
        object actual = flowDefinitionProcessingService
            .ParseFlowDefinitionData(args: "{}");

        // Then
        actual.Should()
            .BeSameAs(expected: expected);

        flowDefinitionServiceMock.VerifyAll();
    }

    [Fact]
    public void ShouldSerializeFlowDefinitionContext()
    {
        // Given
        object context = new();

        flowDefinitionServiceMock
            .Setup(expression: service => service.SerializeContext(
                context: context))
            .Returns(value: "serialized");

        // When
        string actual = flowDefinitionProcessingService
            .SerializeFlowDefinitionContext(context: context);

        // Then
        actual.Should()
            .Be(expected: "serialized");

        flowDefinitionServiceMock.VerifyAll();
    }
}