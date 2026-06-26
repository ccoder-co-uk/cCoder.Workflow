using cCoder.Workflow;
using cCoder.Workflow.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace cCoder.Core.Services.Tests;

public sealed class WorkflowConfigurationEnvironmentTests : IDisposable
{
    private const string CoreEnvironmentKey = "ConnectionStrings__Core";
    private const string DecryptionKeyEnvironmentKey = "Settings__DecryptionKey";
    private const string WorkflowEnvironmentKey = "Services__Workflow";

    private readonly string originalCoreConnectionString;
    private readonly string originalDecryptionKey;
    private readonly string originalWorkflowServiceUrl;

    public WorkflowConfigurationEnvironmentTests()
    {
        originalCoreConnectionString = Environment.GetEnvironmentVariable(CoreEnvironmentKey);
        originalDecryptionKey = Environment.GetEnvironmentVariable(DecryptionKeyEnvironmentKey);
        originalWorkflowServiceUrl = Environment.GetEnvironmentVariable(WorkflowEnvironmentKey);
    }

    [Fact]
    public void AddWorkflowHostedServices_ShouldPopulateBlankConfigurationValuesFromEnvironment()
    {
        // Given
        Environment.SetEnvironmentVariable(CoreEnvironmentKey, "core-from-environment");
        Environment.SetEnvironmentVariable(DecryptionKeyEnvironmentKey, "decryption-from-environment");
        Environment.SetEnvironmentVariable(WorkflowEnvironmentKey, "https://workflow-from-environment.test/");
        ServiceCollection services = new();

        // When
        services.AddWorkflowHostedServices(configuration =>
        {
            configuration.ConnectionStrings["Core"] = string.Empty;
            configuration.Settings["DecryptionKey"] = string.Empty;
            configuration.Services["Workflow"] = string.Empty;
        });

        WorkflowConfiguration configuration = services
            .Single(descriptor => descriptor.ServiceType == typeof(WorkflowConfiguration))
            .ImplementationInstance as WorkflowConfiguration;

        // Then
        configuration.Should().NotBeNull();
        configuration!.ConnectionStrings["Core"].Should().Be("core-from-environment");
        configuration.Settings["DecryptionKey"].Should().Be("decryption-from-environment");
        configuration.Services["Workflow"].Should().Be("https://workflow-from-environment.test/");
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable(CoreEnvironmentKey, originalCoreConnectionString);
        Environment.SetEnvironmentVariable(DecryptionKeyEnvironmentKey, originalDecryptionKey);
        Environment.SetEnvironmentVariable(WorkflowEnvironmentKey, originalWorkflowServiceUrl);
    }
}
