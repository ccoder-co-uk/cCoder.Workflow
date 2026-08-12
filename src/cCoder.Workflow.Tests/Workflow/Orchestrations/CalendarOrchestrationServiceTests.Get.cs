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

public partial class CalendarOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        const int id = 1;
        Calendar entity = CreateRandomCalendar();

        calendarProcessingServiceMock.Setup(expression: x => x.Get(calendarId: id))
            .Returns(value: entity);

        // When
        Calendar result = orchestrationService.Get(calendarId: id);

        // Then
        result.Should()
            .BeSameAs(expected: entity);

        calendarProcessingServiceMock.Verify(expression: x => x.Get(calendarId: id), times: Times.Once);
        calendarProcessingServiceMock.VerifyNoOtherCalls();
        calendarEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}