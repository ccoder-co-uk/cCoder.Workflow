// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Planning;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class CalendarOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDelegateToProcessingServiceWhenDeleteAllAsync()
    {
        // Given
        Calendar[] entities = [CreateRandomCalendar()];

        calendarProcessingServiceMock.Setup(expression: x => x.DeleteAllCalendarAsync(deletedItems: entities))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteAllCalendarAsync(deletedItems: entities);

        // Then
        calendarProcessingServiceMock.Verify(expression: x => x.DeleteAllCalendarAsync(deletedItems: entities), times: Times.Once);
        calendarProcessingServiceMock.VerifyNoOtherCalls();
        calendarEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}