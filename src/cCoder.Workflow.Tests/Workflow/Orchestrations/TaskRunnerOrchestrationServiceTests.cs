// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Services.Processings;
using Moq;

namespace cCoder.Workflow.Services.Orchestrations;

public partial class TaskRunnerOrchestrationServiceTests
{
    private readonly Mock<ICalendarEventProcessingService> calendarEventProcessingServiceMock;
    private readonly Mock<IScheduledTaskProcessingService> scheduledTaskProcessingServiceMock;
    private readonly Mock<IScheduledTaskEventProcessingService> scheduledTaskEventProcessingServiceMock;
    private readonly TaskRunnerOrchestrationService taskRunnerOrchestrationService;

    public TaskRunnerOrchestrationServiceTests()
    {
        calendarEventProcessingServiceMock = new Mock<ICalendarEventProcessingService>();
        scheduledTaskProcessingServiceMock = new Mock<IScheduledTaskProcessingService>();
        scheduledTaskEventProcessingServiceMock = new Mock<IScheduledTaskEventProcessingService>();

        taskRunnerOrchestrationService = new TaskRunnerOrchestrationService(
            scheduledTaskProcessingService: scheduledTaskProcessingServiceMock.Object,
            calendarEventProcessingService: calendarEventProcessingServiceMock.Object,
            scheduledTaskEventProcessingService: scheduledTaskEventProcessingServiceMock.Object);
    }
}