// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Workflow.Services.Orchestrations;
using cCoder.Workflow.Services.Processings;
using Moq;

namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005
public partial class ScheduledTaskOrchestrationServiceTests
{
    private readonly Mock<IScheduledTaskProcessingService> processingServiceMock = new();
    private readonly Mock<IScheduledTaskEventProcessingService> eventServiceMock = new();
    private readonly ScheduledTaskOrchestrationService service;

    public ScheduledTaskOrchestrationServiceTests()
    {
        service = new ScheduledTaskOrchestrationService(
            processingService: processingServiceMock.Object,
            eventService: eventServiceMock.Object);
    }

    private static ScheduledTask CreateScheduledTask() =>
        new()
        {
            Id = Random.Shared.Next(minValue: 1, maxValue: int.MaxValue),
            AppId = 7,
            Name = "Task"
        };
}
#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005