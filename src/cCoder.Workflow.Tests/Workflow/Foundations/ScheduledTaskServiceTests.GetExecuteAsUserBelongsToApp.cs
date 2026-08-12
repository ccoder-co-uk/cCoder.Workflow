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
    public void ShouldGetExecuteAsUserBelongsToApp()
    {
        // Given
        const string executeAs = "user";
        const int appId = 7;

        scheduledTaskBrokerMock
            .Setup(expression: broker => broker
                .SelectExecuteAsUserBelongsToApp(
                    executeAs: executeAs,
                    appId: appId))
            .Returns(value: true);

        // When
        bool actual = scheduledTaskService.GetExecuteAsUserBelongsToApp(
            executeAs: executeAs,
            appId: appId);

        // Then
        actual
            .Should()
            .BeTrue();

        scheduledTaskBrokerMock.VerifyAll();
    }
}