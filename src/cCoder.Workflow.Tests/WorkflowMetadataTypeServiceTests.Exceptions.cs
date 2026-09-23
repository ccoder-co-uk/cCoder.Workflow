// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations.Schema;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests;

public partial class WorkflowMetadataTypeServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        Workflow.Foundations.WorkflowScriptExecutionServiceTests
            .ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void GetCoreMetadata_WhenReflectionFails_MapsException(
        Exception exception,
        Type expectedType)
    {
        // Given
        var reflectionBrokerMock = new Mock<IReflectionBroker>(
            behavior: MockBehavior.Strict);

        reflectionBrokerMock
            .Setup(expression: broker =>
                broker.GetCustomAttribute<TableAttribute>(
                    member: It.IsAny<System.Reflection.MemberInfo>()))
            .Throws(exception: exception);

        var metadataService = new WorkflowMetadataTypeService(
            reflectionBroker: reflectionBrokerMock.Object);

        // When
        Action action = () => metadataService.GetCoreMetadata();

        // Then
        Exception thrown = action
            .Should()
            .Throw<Exception>()
            .Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}