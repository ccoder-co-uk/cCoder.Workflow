using cCoder.Workflow;
using cCoder.Workflow.Exposures.EventHandlers;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace cCoder.Core.Services.Tests;

public sealed class WebApplicationExtensionsTests
{
    [Fact]
    public async Task StartWorkflowHostedServices_ShouldOnlyRegisterHandlersOnceWhenCalledMultipleTimes()
    {
        // Given
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        TestWorkflowEventHandlers handlers = new();
        builder.Services.AddLogging();
        builder.Services.AddSingleton<IWorkflowEventHandlers>(handlers);
        await using WebApplication app = builder.Build();

        // When
        app.StartWorkflowHostedServices();
        app.StartWorkflowHostedServices();

        // Then
        handlers.ListenToAllEventsCallCount.Should().Be(1);
        handlers.ListenToScheduledTaskExecuteEventsCallCount.Should().Be(1);
        handlers.ListenToQueuedFlowInstanceExecuteEventsCallCount.Should().Be(1);
    }

    private sealed class TestWorkflowEventHandlers : IWorkflowEventHandlers
    {
        public int ListenToAllEventsCallCount { get; private set; }

        public int ListenToScheduledTaskExecuteEventsCallCount { get; private set; }

        public int ListenToQueuedFlowInstanceExecuteEventsCallCount { get; private set; }

        public void ListenToAllEvents() => ListenToAllEventsCallCount++;

        public void ListenToScheduledTaskExecuteEvents() =>
            ListenToScheduledTaskExecuteEventsCallCount++;

        public void ListenToQueuedFlowInstanceExecuteEvents() =>
            ListenToQueuedFlowInstanceExecuteEventsCallCount++;
    }
}
