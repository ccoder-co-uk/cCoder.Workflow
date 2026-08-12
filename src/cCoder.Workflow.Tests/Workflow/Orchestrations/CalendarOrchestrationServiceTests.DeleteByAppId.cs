// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class CalendarOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDeleteCalendarEventsBeforeCalendarsByAppIdAsync()
    {
        // Given
        calendarEventProcessingServiceMock
            .Setup(expression: service => service.DeleteAllByAppIdAsync(appId: 7))
            .Returns(value: ValueTask.CompletedTask);

        calendarProcessingServiceMock
            .Setup(expression: service => service.DeleteByAppIdAsync(appId: 7))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.DeleteByAppIdAsync(appId: 7);

        // Then
        calendarEventProcessingServiceMock.VerifyAll();
        calendarProcessingServiceMock.VerifyAll();
    }
}