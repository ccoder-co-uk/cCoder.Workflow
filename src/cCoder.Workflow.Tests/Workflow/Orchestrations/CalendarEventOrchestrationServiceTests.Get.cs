// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Planning;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class CalendarEventOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        const int id = 1;
        CalendarEvent entity = CreateRandomCalendarEvent();

        calendarEventProcessingServiceMock.Setup(expression: x => x.Get(calendarEventId: id))
            .Returns(value: entity);

        // When
        CalendarEvent result = orchestrationService.Get(calendarEventId: id);

        // Then
        result.Should()
            .BeSameAs(expected: entity);

        calendarEventProcessingServiceMock.Verify(expression: x => x.Get(calendarEventId: id), times: Times.Once);
        calendarEventProcessingServiceMock.VerifyNoOtherCalls();
        calendarEventEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}