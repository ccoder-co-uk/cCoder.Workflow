// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class ScheduledTaskServiceTests
{
    [Fact]
    public async Task ShouldDeleteAllByAppIdAsync()
    {
        // Given
        const int appId = 7;

        scheduledTaskBrokerMock
            .Setup(expression: broker => broker
                .DeleteAllScheduledTasksByAppIdAsync(appId: appId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await scheduledTaskService.DeleteAllByAppIdAsync(appId: appId);

        // Then
        scheduledTaskBrokerMock.VerifyAll();
    }
}