// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers;
using cCoder.Workflow.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests.Workflow.Foundations;

public sealed partial class WorkflowInstanceManagementServiceTests
{
    public static TheoryData<Exception, Type> ExceptionMappings =>
        WorkflowScriptExecutionServiceTests.ExceptionMappings;

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void GetFailedExecutionStats_WhenBrokerFails_MapsException(
        Exception exception,
        Type expectedType)
    {
        // Given
        var brokerMock = new Mock<IWorkflowInstanceManagementBroker>(
            behavior: MockBehavior.Strict);

        brokerMock
            .Setup(expression: broker => broker.GetFailedExecutionStats())
            .Throws(exception: exception);

        var service = new WorkflowInstanceManagementService(
            workflowInstanceManagementBroker: brokerMock.Object);

        // When
        Action action = () => service.GetFailedExecutionStats();

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