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
    public async Task ShouldPassThroughCallWhenRaiseCalendarEventDeleteEventAsync()
    {
        // Given
        CalendarEvent entity = CreateRandomCalendarEvent();

        calendarEventEventServiceMock
            .Setup(expression: x => x.RaiseCalendarEventDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseCalendarEventDeleteEventAsync(entity: entity);

        // Then
        calendarEventEventServiceMock.Verify(expression: x => x.RaiseCalendarEventDeleteEventAsync(entity: entity), times: Times.Once);
        calendarEventEventServiceMock.VerifyNoOtherCalls();
    }

}