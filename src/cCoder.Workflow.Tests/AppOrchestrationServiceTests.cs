// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Orchestrations;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests;

public class AppOrchestrationServiceTests
{
    private readonly Mock<ICalendarOrchestrationService> calendarOrchestrationServiceMock;
    private readonly Mock<IFlowDefinitionOrchestrationService> flowDefinitionOrchestrationServiceMock;
    private readonly Mock<IScheduledTaskOrchestrationService> scheduledTaskOrchestrationServiceMock;
    private readonly AppOrchestrationService service;

    public AppOrchestrationServiceTests()
    {
        calendarOrchestrationServiceMock = new Mock<ICalendarOrchestrationService>(MockBehavior.Strict);
        flowDefinitionOrchestrationServiceMock = new Mock<IFlowDefinitionOrchestrationService>(MockBehavior.Strict);
        scheduledTaskOrchestrationServiceMock = new Mock<IScheduledTaskOrchestrationService>(MockBehavior.Strict);
        service = new AppOrchestrationService(
            flowDefinitionOrchestrationServiceMock.Object,
            calendarOrchestrationServiceMock.Object,
            scheduledTaskOrchestrationServiceMock.Object);
    }

    [Fact]
    public async Task ShouldDeleteAppOwnedFlowsByAppIdWhenDeleteAsync()
    {
        scheduledTaskOrchestrationServiceMock.Setup(expression:x => x.DeleteByAppIdAsync(5))
            .Returns(value:ValueTask.CompletedTask);
        calendarOrchestrationServiceMock.Setup(expression:x => x.DeleteByAppIdAsync(5))
            .Returns(value:ValueTask.CompletedTask);
        flowDefinitionOrchestrationServiceMock.Setup(expression:x => x.DeleteByAppIdAsync(5))
            .Returns(value:ValueTask.CompletedTask);

        await service.DeleteAsync(appId:5);

        scheduledTaskOrchestrationServiceMock.Verify(expression:x => x.DeleteByAppIdAsync(5), times:Times.Once);
        calendarOrchestrationServiceMock.Verify(expression:x => x.DeleteByAppIdAsync(5), times:Times.Once);
        flowDefinitionOrchestrationServiceMock.Verify(expression:x => x.DeleteByAppIdAsync(5), times:Times.Once);
        scheduledTaskOrchestrationServiceMock.VerifyNoOtherCalls();
        calendarOrchestrationServiceMock.VerifyNoOtherCalls();
        flowDefinitionOrchestrationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldStampFlowAppIdsWhenAddAsync()
    {
        App app = new()
        {
            Id = 9,
            Calendars = [new Calendar { Name = "Calendar" }],
            Flows = [new FlowDefinition { Id = Guid.NewGuid(), Name = "Flow" }],
            Tasks = [new ScheduledTask { Name = "Task" }],
        };

        calendarOrchestrationServiceMock.Setup(expression:x => x.AddOrUpdate(
                It.Is<IEnumerable<Calendar>>(items => items.All(calendar => calendar.AppId == 9))))
            .Returns(value:ValueTask.FromResult<IEnumerable<cCoder.Workflow.Models.Result<Calendar>>>([]));
        flowDefinitionOrchestrationServiceMock.Setup(expression:x => x.AddOrUpdate(
                It.Is<IEnumerable<FlowDefinition>>(items => items.All(flow => flow.AppId == 9))))
            .Returns(value:ValueTask.FromResult<IEnumerable<cCoder.Workflow.Models.Result<FlowDefinition>>>([]));
        scheduledTaskOrchestrationServiceMock.Setup(expression:x => x.AddOrUpdate(
                It.Is<IEnumerable<ScheduledTask>>(items => items.All(task => task.AppId == 9))))
            .Returns(value:ValueTask.FromResult<IEnumerable<cCoder.Workflow.Models.Result<ScheduledTask>>>([]));

        await service.AddAsync(app:app);

        calendarOrchestrationServiceMock.VerifyAll();
        flowDefinitionOrchestrationServiceMock.VerifyAll();
        scheduledTaskOrchestrationServiceMock.VerifyAll();
    }
}