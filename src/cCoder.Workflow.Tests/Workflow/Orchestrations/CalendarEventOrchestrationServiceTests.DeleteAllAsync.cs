// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Planning;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class CalendarEventOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        CalendarEvent[] entities = [CreateRandomCalendarEvent()];

        calendarEventProcessingServiceMock.Setup(expression: x => x.DeleteAllCalendarEventAsync(deletedItems: entities))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllCalendarEventAsync(deletedItems: entities);

        // Then
        calendarEventProcessingServiceMock.Verify(expression: x => x.DeleteAllCalendarEventAsync(deletedItems: entities), times: Times.Once);
        calendarEventProcessingServiceMock.VerifyNoOtherCalls();
        calendarEventEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}