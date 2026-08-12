// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class FlowDefinitionServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        new()
        {
            {
                new WorkflowValidationException(
                    innerException: new ArgumentException()),
                typeof(WorkflowValidationException)
            },
            {
                new WorkflowDependencyException(
                    innerException: new InvalidOperationException()),
                typeof(WorkflowDependencyException)
            },
            { new ValidationException(), typeof(WorkflowValidationException) },
            { new InvalidOperationException(), typeof(WorkflowDependencyException) },
            { new SecurityException(), typeof(SecurityException) },
            { new Exception(), typeof(WorkflowServiceException) }
        };

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetAllFailure(Exception exception, Type expectedType)
    {
        // Given
        flowDefinitionBrokerMock
            .Setup(expression: broker => broker.SelectAllFlowDefinitions())
            .Throws(exception: exception);

        // When
        Action action = () => flowDefinitionService.GetAll();

        // Then
        action
            .Should()
            .Throw<Exception>()
            .Which
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapAddFlowDefinitionAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        FlowDefinition flowDefinition = CreateRandomFlowDefinition();

        authorizationBrokerMock
            .Setup(expression: broker => broker.Authorize(
                appId: flowDefinition.AppId,
                privilege: "FlowDefinition_create"))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await flowDefinitionService
            .AddFlowDefinitionAsync(newFlowDefinition: flowDefinition);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapDeleteWithInstancesByAppIdAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        flowDefinitionBrokerMock
            .Setup(expression: broker => broker
                .DeleteFlowDefinitionsWithInstancesByAppIdAsync(appId: 7))
            .Throws(exception: exception);

        // When
        Func<Task> action = async () => await flowDefinitionService
            .DeleteWithInstancesByAppIdAsync(appId: 7);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}