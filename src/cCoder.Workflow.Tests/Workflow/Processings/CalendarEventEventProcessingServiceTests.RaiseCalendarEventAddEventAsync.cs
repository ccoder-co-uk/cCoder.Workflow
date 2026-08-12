// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Planning;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class CalendarEventEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseCalendarEventAddEventAsync()
    {
        // Given
        CalendarEvent entity = CreateRandomCalendarEvent();

        calendarEventEventServiceMock
            .Setup(expression: x => x.RaiseCalendarEventAddEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseCalendarEventAddEventAsync(entity: entity);

        // Then
        calendarEventEventServiceMock.Verify(expression: x => x.RaiseCalendarEventAddEventAsync(entity: entity), times: Times.Once);
        calendarEventEventServiceMock.VerifyNoOtherCalls();
    }

}