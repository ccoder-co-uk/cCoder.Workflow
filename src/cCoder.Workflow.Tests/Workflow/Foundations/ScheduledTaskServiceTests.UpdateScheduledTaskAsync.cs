// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class ScheduledTaskServiceTests
{
    [Fact]
    public async Task ShouldUpdateScheduledTaskAsync()
    {
        // Given
        ScheduledTask input = CreateScheduledTask();
        ScheduledTask stored = CreateScheduledTask();
        const string userId = "user";

        authorizationBrokerMock
            .Setup(expression: broker => broker.IsAdminOfApp(
                appId: input.AppId))
            .Returns(value: true);

        scheduledTaskBrokerMock
            .Setup(expression: broker => broker
                .SelectExecuteAsUserBelongsToApp(
                    executeAs: input.ExecuteAs,
                    appId: input.AppId))
            .Returns(value: true);

        scheduledTaskBrokerMock
            .Setup(expression: broker => broker.SelectFlowBelongsToApp(
                flowId: input.FlowId,
                appId: input.AppId))
            .Returns(value: true);

        authorizationBrokerMock
            .Setup(expression: broker => broker.Authorize(
                appId: input.AppId,
                privilege: "ScheduledTask_update"));

        authorizationBrokerMock
            .Setup(expression: broker => broker.GetCurrentUser())
            .Returns(value: new User { Id = userId });

        scheduledTaskBrokerMock
            .Setup(expression: broker => broker.UpdateScheduledTaskAsync(
                updatedEntity: It.Is<ScheduledTask>(match: updated =>
                    updated.Id == input.Id
                    && updated.UpdatedBy == userId)))
            .Returns(value: ValueTask.FromResult(result: stored));

        // When
        ScheduledTask actual = await scheduledTaskService
            .UpdateScheduledTaskAsync(updatedScheduledTask: input);

        // Then
        actual
            .Should()
            .BeSameAs(expected: input);

        actual.Id
            .Should()
            .Be(expected: stored.Id);

        actual.FlowId
            .Should()
            .Be(expected: stored.FlowId);

        authorizationBrokerMock.VerifyAll();
        scheduledTaskBrokerMock.VerifyAll();
    }
}