// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Security;
using cCoder.Workflow.Models.Exceptions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests.Workflow.Coordinations;

public sealed partial class FlowDefinitionManagementCoordinationServiceTests
{
    public static TheoryData<Exception, Type> DependencyExceptions =>
        new()
        {
            {
                new WorkflowValidationException(
                    innerException: new Exception()),
                typeof(WorkflowValidationException)
            },
            {
                new WorkflowDependencyException(
                    innerException: new Exception()),
                typeof(WorkflowDependencyException)
            },
            {
                new ValidationException(),
                typeof(WorkflowValidationException)
            },
            {
                new InvalidOperationException(),
                typeof(WorkflowDependencyException)
            },
            {
                new SecurityException(),
                typeof(SecurityException)
            },
            {
                new Exception(),
                typeof(WorkflowServiceException)
            }
        };

    [Theory]
    [MemberData(nameof(DependencyExceptions))]
    public void GetFlowDefinition_WhenDependencyFails_MapsException(
        Exception dependencyException,
        Type expectedExceptionType)
    {
        // Given
        Guid flowDefinitionId = Guid.NewGuid();

        flowDefinitionOrchestrationServiceMock
            .Setup(expression: service => service.Get(
                flowDefinitionId: flowDefinitionId))
            .Throws(exception: dependencyException);

        // When
        Action action = () => flowDefinitionManagementCoordinationService
            .GetFlowDefinition(flowDefinitionId: flowDefinitionId);

        // Then
        Exception actualException = Assert.ThrowsAny<Exception>(
            testCode: action);

        Assert.IsType(
            expectedType: expectedExceptionType,
            @object: actualException);
    }

    [Theory]
    [MemberData(nameof(DependencyExceptions))]
    public async Task AddFlowDefinition_WhenDependencyFails_MapsException(
        Exception dependencyException,
        Type expectedExceptionType)
    {
        // Given
        var flowDefinition = CreateFlowDefinition();

        flowDefinitionOrchestrationServiceMock
            .Setup(expression: service => service.AddFlowDefinitionAsync(
                newFlowDefinition: flowDefinition))
            .Throws(exception: dependencyException);

        // When
        Func<Task> action = async () =>
            await flowDefinitionManagementCoordinationService
                .AddFlowDefinitionAsync(
                    newFlowDefinition: flowDefinition);

        // Then
        Exception actualException = await Assert.ThrowsAnyAsync<Exception>(
            testCode: action);

        Assert.IsType(
            expectedType: expectedExceptionType,
            @object: actualException);
    }

    [Theory]
    [MemberData(nameof(DependencyExceptions))]
    public async Task DeleteFlowDefinition_WhenDependencyFails_MapsException(
        Exception dependencyException,
        Type expectedExceptionType)
    {
        // Given
        Guid flowDefinitionId = Guid.NewGuid();

        flowDefinitionOrchestrationServiceMock
            .Setup(expression: service => service.DeleteAsync(
                flowDefinitionId: flowDefinitionId))
            .Throws(exception: dependencyException);

        // When
        Func<Task> action = async () =>
            await flowDefinitionManagementCoordinationService
                .DeleteFlowDefinitionAsync(
                    flowDefinitionId: flowDefinitionId);

        // Then
        Exception actualException = await Assert.ThrowsAnyAsync<Exception>(
            testCode: action);

        Assert.IsType(
            expectedType: expectedExceptionType,
            @object: actualException);
    }
}