// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Exposures;
using cCoder.Workflow.Engine.Services.Processings;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class IServiceCollectionExtensionsTests
{
    [Fact]
    public void AddWorkflowEngine_RegistersResolvableEngineServices()
    {
        // Given

        // When
        services.AddWorkflowEngine();

        // Then
        using ServiceProvider serviceProvider = services.BuildServiceProvider(
options: new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });

        serviceProvider.GetRequiredService<IFlowRunner>()
            .Should()
            .NotBeNull();

        serviceProvider.GetRequiredService<IWorkflowScriptExecutionService>()
            .Should()
            .NotBeNull();

        serviceProvider.GetRequiredService<IScriptProcessingService>()
            .Should()
            .NotBeNull();
    }
}