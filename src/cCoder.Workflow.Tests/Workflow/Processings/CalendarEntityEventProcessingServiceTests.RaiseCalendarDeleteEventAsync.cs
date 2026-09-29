// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Planning;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class CalendarEntityEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseCalendarDeleteEventAsync()
    {
        // Given
        Calendar entity = CreateRandomCalendar();

        calendarEntityEventServiceMock
            .Setup(expression: x => x.RaiseCalendarDeleteEventAsync(calendar: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseCalendarDeleteEventAsync(calendar: entity);

        // Then
        calendarEntityEventServiceMock.Verify(expression: x => x.RaiseCalendarDeleteEventAsync(calendar: entity), times: Times.Once);
        calendarEntityEventServiceMock.VerifyNoOtherCalls();
    }

}