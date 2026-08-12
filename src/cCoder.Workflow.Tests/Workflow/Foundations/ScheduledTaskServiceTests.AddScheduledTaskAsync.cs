// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class ScheduledTaskServiceTests
{
    [Fact]
    public async Task ShouldAddScheduledTaskAsync()
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
                privilege: "ScheduledTask_create"));

        authorizationBrokerMock
            .Setup(expression: broker => broker.GetCurrentUser())
            .Returns(value: new User { Id = userId });

        scheduledTaskBrokerMock
            .Setup(expression: broker => broker.InsertScheduledTaskAsync(
                newEntity: It.Is<ScheduledTask>(match: added =>
                    added.Name == input.Name
                    && added.CreatedBy == userId
                    && added.UpdatedBy == userId)))
            .Returns(value: ValueTask.FromResult(result: stored));

        // When
        ScheduledTask actual = await scheduledTaskService
            .AddScheduledTaskAsync(newScheduledTask: input);

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

    [Theory]
    [InlineData(false, true, true)]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    public async Task ShouldRejectUnauthorizedScheduledTaskOnAddAsync(
        bool isAppAdmin,
        bool userBelongsToApp,
        bool flowBelongsToApp)
    {
        // Given
        ScheduledTask input = CreateScheduledTask();

        authorizationBrokerMock
            .Setup(expression: broker => broker.IsAdminOfApp(
                appId: input.AppId))
            .Returns(value: isAppAdmin);

        scheduledTaskBrokerMock
            .Setup(expression: broker => broker
                .SelectExecuteAsUserBelongsToApp(
                    executeAs: input.ExecuteAs,
                    appId: input.AppId))
            .Returns(value: userBelongsToApp);

        scheduledTaskBrokerMock
            .Setup(expression: broker => broker.SelectFlowBelongsToApp(
                flowId: input.FlowId,
                appId: input.AppId))
            .Returns(value: flowBelongsToApp);

        // When
        Func<Task> action = async () => await scheduledTaskService
            .AddScheduledTaskAsync(newScheduledTask: input);

        // Then
        await action
            .Should()
            .ThrowAsync<SecurityException>();

        authorizationBrokerMock.VerifyAll();
        scheduledTaskBrokerMock.VerifyAll();
    }
}