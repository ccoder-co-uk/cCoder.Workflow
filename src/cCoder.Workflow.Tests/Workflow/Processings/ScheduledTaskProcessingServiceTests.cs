// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Workflow.Brokers.Loggings;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Foundations;
using cCoder.Workflow.Services.Processings;
using Moq;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

#pragma warning disable STXFORMAT008
public partial class ScheduledTaskProcessingServiceTests
{
    private readonly Mock<IScheduledTaskService> scheduledTaskServiceMock = new();

    private readonly Mock<ILoggingBroker> loggingBrokerMock = new();

    private readonly WorkflowConfiguration configuration = new();

    private readonly ScheduledTaskProcessingService processingService;

    public ScheduledTaskProcessingServiceTests()
    {
        processingService = new ScheduledTaskProcessingService(
            service: scheduledTaskServiceMock.Object,
            configuration: configuration,
            logger: loggingBrokerMock.Object);
    }

    private static ScheduledTask CreateScheduledTask() =>
        new()
        {
            Id = Random.Shared.Next(minValue: 1, maxValue: int.MaxValue),
            AppId = Random.Shared.Next(minValue: 1, maxValue: int.MaxValue),
            FlowId = Guid.NewGuid(),
            Name = "Task"
        };
}
#pragma warning restore STXFORMAT008