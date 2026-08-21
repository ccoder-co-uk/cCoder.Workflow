// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class ScheduledTaskProcessingServiceTests
{
    [Fact]
    public void GetScheduledTaskPollingIntervalShouldReturnProductionDefaultWhenNotConfigured()
    {
        // Given

        // When
        TimeSpan actualInterval = processingService.GetScheduledTaskPollingInterval();

        // Then
        actualInterval.Should()
            .Be(expected: TimeSpan.FromMinutes(minutes: 1));
    }

    [Fact]
    public void GetScheduledTaskPollingIntervalShouldReturnConfiguredInterval()
    {
        // Given
        configuration.ScheduledTaskPollingIntervalMilliseconds = 250;

        // When
        TimeSpan actualInterval = processingService.GetScheduledTaskPollingInterval();

        // Then
        actualInterval.Should()
            .Be(expected: TimeSpan.FromMilliseconds(milliseconds: 250));
    }
}