// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Coordinations;
using cCoder.Workflow.Services.Orchestrations;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests;

public partial class AppCoordinationServiceTests
{
    private readonly Mock<ICalendarOrchestrationService> calendarOrchestrationServiceMock;
    private readonly Mock<IFlowDefinitionOrchestrationService> flowDefinitionOrchestrationServiceMock;
    private readonly Mock<IScheduledTaskOrchestrationService> scheduledTaskOrchestrationServiceMock;
    private readonly AppCoordinationService service;

    public AppCoordinationServiceTests()
    {
        calendarOrchestrationServiceMock = new Mock<ICalendarOrchestrationService>(behavior: MockBehavior.Strict);
        flowDefinitionOrchestrationServiceMock = new Mock<IFlowDefinitionOrchestrationService>(behavior: MockBehavior.Strict);
        scheduledTaskOrchestrationServiceMock = new Mock<IScheduledTaskOrchestrationService>(behavior: MockBehavior.Strict);
        service = new AppCoordinationService(
            flowDefinitionOrchestrationServiceMock.Object,
            calendarOrchestrationServiceMock.Object,
            scheduledTaskOrchestrationServiceMock.Object);
    }

    [Fact]
    public async Task ShouldDeleteAppOwnedFlowsByAppIdWhenDeleteAsync()
    {
        // Given
        scheduledTaskOrchestrationServiceMock.Setup(expression: x => x.DeleteByAppIdAsync(appId: 5))
            .Returns(value: ValueTask.CompletedTask);

        calendarOrchestrationServiceMock.Setup(expression: x => x.DeleteByAppIdAsync(appId: 5))
            .Returns(value: ValueTask.CompletedTask);

        flowDefinitionOrchestrationServiceMock.Setup(expression: x => x.DeleteByAppIdAsync(appId: 5))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.DeleteAsync(appId: 5);

        // Then
        scheduledTaskOrchestrationServiceMock.Verify(expression: x => x.DeleteByAppIdAsync(appId: 5), times: Times.Once);
        calendarOrchestrationServiceMock.Verify(expression: x => x.DeleteByAppIdAsync(appId: 5), times: Times.Once);
        flowDefinitionOrchestrationServiceMock.Verify(expression: x => x.DeleteByAppIdAsync(appId: 5), times: Times.Once);
        scheduledTaskOrchestrationServiceMock.VerifyNoOtherCalls();
        calendarOrchestrationServiceMock.VerifyNoOtherCalls();
        flowDefinitionOrchestrationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldStampFlowAppIdsWhenAddAsync()
    {
        // Given
        App app = new()
        {
            Id = 9,
            Calendars = [new Calendar { Name = "Calendar" }],
            Flows = [new FlowDefinition { Id = Guid.NewGuid(), Name = "Flow" }],
            Tasks = [new ScheduledTask { Name = "Task" }],
        };

        calendarOrchestrationServiceMock.Setup(expression: x => x.AddOrUpdateCalendar(
items: It.Is<IEnumerable<Calendar>>(match: items => items.All(predicate: calendar => calendar.AppId == 9))))
            .Returns(value: ValueTask.FromResult<IEnumerable<Result<Calendar>>>(result: []));

        flowDefinitionOrchestrationServiceMock.Setup(expression: x => x.AddOrUpdateFlowDefinition(
items: It.Is<IEnumerable<FlowDefinition>>(match: items => items.All(predicate: flow => flow.AppId == 9))))
            .Returns(value: ValueTask.FromResult<IEnumerable<Result<FlowDefinition>>>(result: []));

        scheduledTaskOrchestrationServiceMock.Setup(expression: x => x.AddOrUpdateScheduledTask(
items: It.Is<IEnumerable<ScheduledTask>>(match: items => items.All(predicate: task => task.AppId == 9))))
            .Returns(value: ValueTask.FromResult<IEnumerable<Result<ScheduledTask>>>(result: []));

        // When
        await service.AddAppAsync(newApp: app);

        // Then
        calendarOrchestrationServiceMock.VerifyAll();
        flowDefinitionOrchestrationServiceMock.VerifyAll();
        scheduledTaskOrchestrationServiceMock.VerifyAll();
    }
}