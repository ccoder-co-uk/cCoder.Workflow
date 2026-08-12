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

public partial class CalendarEntityEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseCalendarUpdateEventAsync()
    {
        // Given
        Calendar entity = CreateRandomCalendar();

        calendarEntityEventServiceMock
            .Setup(expression: x => x.RaiseCalendarUpdateEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseCalendarUpdateEventAsync(entity: entity);

        // Then
        calendarEntityEventServiceMock.Verify(expression: x => x.RaiseCalendarUpdateEventAsync(entity: entity), times: Times.Once);
        calendarEntityEventServiceMock.VerifyNoOtherCalls();
    }

}