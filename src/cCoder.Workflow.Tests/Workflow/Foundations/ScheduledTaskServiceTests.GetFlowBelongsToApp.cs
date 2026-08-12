// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class ScheduledTaskServiceTests
{
    [Fact]
    public void ShouldGetFlowBelongsToApp()
    {
        // Given
        Guid flowId = Guid.NewGuid();
        const int appId = 7;

        scheduledTaskBrokerMock
            .Setup(expression: broker => broker.SelectFlowBelongsToApp(
                flowId: flowId,
                appId: appId))
            .Returns(value: true);

        // When
        bool actual = scheduledTaskService.GetFlowBelongsToApp(
            flowId: flowId,
            appId: appId);

        // Then
        actual
            .Should()
            .BeTrue();

        scheduledTaskBrokerMock.VerifyAll();
    }
}