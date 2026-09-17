// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow;
using cCoder.Eventing;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests;

public sealed partial class WebApplicationExtensionsTests
{
    [Fact]
    public async Task StartWorkflowWeb_ShouldNotRegisterEventOrEngineExecutionHandlers()
    {
        // Given
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        Mock<IEventHub> eventHubMock = new();
        builder.Services.AddLogging();
        builder.Services.AddSignalR();
        builder.Services.AddSingleton<IEventHub>(implementationInstance: eventHubMock.Object);
        builder.Services.AddSingleton<cCoder.Data.Exposures.IMetadataTypeCache, TestMetadataTypeCache>();

        builder.Services.AddSingleton<cCoder.Workflow.Services.Foundations.IWorkflowMetadataTypeService>(
implementationInstance: new MockWorkflowMetadataTypeService());

        await using WebApplication app = builder.Build();

        // When
        app.StartWorkflowWeb();

        // Then
        eventHubMock.Invocations.Should()
            .BeEmpty();
    }

    private sealed class TestMetadataTypeCache : cCoder.Data.Exposures.IMetadataTypeCache
    {
        private readonly HashSet<string> keys = [];

        public bool Contains(string key) =>
            keys.Contains(item: key);

        public string[] Get(string key) =>
            [];

        public string[] GetAll() =>
            [];

        public void Clear(string key) =>
            keys.Remove(item: key);

        public void Set(string key, IEnumerable<string> values) =>
            keys.Add(item: key);
    }

    private sealed class MockWorkflowMetadataTypeService
        : cCoder.Workflow.Services.Foundations.IWorkflowMetadataTypeService
    {
        public cCoder.Workflow.Models.OData.MetadataContainerSet GetCoreMetadata() =>
            new();

        public cCoder.Workflow.Models.OData.MetadataContainerSet[] GetKnownActivityTypes() =>
            [];

        public cCoder.Workflow.Models.OData.MetadataContainerSet[] GetKnownSystemTypes() =>
            [];

        public cCoder.Workflow.Models.OData.MetadataContainerSet GetSharedMetadata() =>
            new();
    }
}