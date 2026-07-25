// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Brokers.Storage;
using cCoder.Workflow.Services.Foundations;
using Moq;

namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class ScheduledTaskServiceTests
{
    private readonly Mock<IScheduledTaskBroker> scheduledTaskBrokerMock;
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock;
    private readonly ScheduledTaskService scheduledTaskService;

    public ScheduledTaskServiceTests()
    {
        scheduledTaskBrokerMock =
            new Mock<IScheduledTaskBroker>(behavior: MockBehavior.Strict);

        authorizationBrokerMock =
            new Mock<IAuthorizationBroker>(behavior: MockBehavior.Strict);

        scheduledTaskService = new ScheduledTaskService(
            scheduledTaskBroker: scheduledTaskBrokerMock.Object,
            authorizationBroker: authorizationBrokerMock.Object);
    }

    private static ScheduledTask CreateScheduledTask() =>
        new()
        {
            Id = Random.Shared.Next(minValue: 1, maxValue: int.MaxValue),
            AppId = Random.Shared.Next(minValue: 1, maxValue: int.MaxValue),
            LastExecuted = DateTimeOffset.UtcNow.AddDays(days: -1),
            NextExecution = DateTimeOffset.UtcNow.AddMinutes(minutes: -1),
            ScheduleInTicks = TimeSpan.FromMinutes(minutes: 5).Ticks
        };
}