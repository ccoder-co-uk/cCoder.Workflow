// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        builder.Services.AddSingleton<IWorkflowEventHandlers>(implementationInstance:handlers);
        await using WebApplication app = builder.Build();

        // When
        app.StartWorkflowHostedServices();
        app.StartWorkflowHostedServices();

        // Then
        handlers.ListenToAllEventsCallCount.Should().Be(expected:1);
        handlers.ListenToScheduledTaskExecuteEventsCallCount.Should().Be(expected:1);
        handlers.ListenToQueuedFlowInstanceExecuteEventsCallCount.Should().Be(expected:1);
    }

    [Fact]
    public async Task StartWorkflowWeb_ShouldNotRegisterEventOrEngineExecutionHandlers()
    {
        // Given
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        TestWorkflowEventHandlers handlers = new();
        builder.Services.AddLogging();
        builder.Services.AddSignalR();
        builder.Services.AddSingleton<IWorkflowEventHandlers>(implementationInstance:handlers);
        builder.Services.AddSingleton<cCoder.Data.Exposures.IMetadataTypeCache, TestMetadataTypeCache>();
        builder.Services.AddSingleton<cCoder.Workflow.Services.Foundations.IWorkflowMetadataTypeService>(
implementationInstance:            new MockWorkflowMetadataTypeService());
        await using WebApplication app = builder.Build();

        // When
        app.StartWorkflowWeb();

        // Then
        handlers.ListenToAllEventsCallCount.Should().Be(expected:0);
        handlers.ListenToScheduledTaskExecuteEventsCallCount.Should().Be(expected:0);
        handlers.ListenToQueuedFlowInstanceExecuteEventsCallCount.Should().Be(expected:0);
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

    private sealed class TestMetadataTypeCache : cCoder.Data.Exposures.IMetadataTypeCache
    {
        private readonly HashSet<string> keys = [];

        public bool Contains(string key) => keys.Contains(item:key);

        public string[] Get(string key) => [];

        public string[] GetAll() => [];

        public void Clear(string key) => keys.Remove(item:key);

        public void Set(string key, IEnumerable<string> values) => keys.Add(item:key);
    }

    private sealed class MockWorkflowMetadataTypeService
        : cCoder.Workflow.Services.Foundations.IWorkflowMetadataTypeService
    {
        public cCoder.Workflow.Api.OData.MetadataContainerSet GetCoreMetadata() => new();

        public cCoder.Workflow.Api.OData.MetadataContainerSet[] GetKnownActivityTypes() => [];

        public cCoder.Workflow.Api.OData.MetadataContainerSet[] GetKnownSystemTypes() => [];

        public cCoder.Workflow.Api.OData.MetadataContainerSet GetSharedMetadata() => new();
    }
}