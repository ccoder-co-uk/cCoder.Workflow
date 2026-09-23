// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;
using cCoder.Workflow.Exposures.HostedServices;
using cCoder.Workflow.Services.Aggregations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;


namespace cCoder.Workflow.Tests;

public partial class HostedServicesRegistrationTests
{
    [Fact]
    public void AddWorkflowWeb_DoesNotRegisterHostedServiceExposures()
    {
        // Given
        IServiceCollection services = new ServiceCollection();

        // When
        services.AddWorkflowWeb();

        // Then
        Assert.DoesNotContain(
collection: services,
filter: descriptor => descriptor.ServiceType == typeof(IHostedService)
                && descriptor.ImplementationFactory is not null);
    }

    [Fact]
    public void AddWorkflowHostedServices_RegistersHostedServiceExposures()
    {
        // Given
        IServiceCollection services = new ServiceCollection();

        // When
        services.AddWorkflowHostedServices();

        // Then
        Assert.Contains(
collection: services,
filter: descriptor => descriptor.ServiceType == typeof(IHostedService)
                && descriptor.ImplementationType == typeof(InstanceMaintenanceBackgroundService));

        Assert.Contains(
collection: services,
filter: descriptor => descriptor.ServiceType == typeof(IHostedService)
                && descriptor.ImplementationType == typeof(QueueInstanceBackgroundService));

        Assert.Contains(
collection: services,
filter: descriptor => descriptor.ServiceType == typeof(IHostedService)
                && descriptor.ImplementationType == typeof(ScheduledTaskRunnerBackgroundService));

        Assert.Equal(
expected: 3,
actual: services.Count(predicate: descriptor => descriptor.ServiceType == typeof(IHostedService)
                && descriptor.ImplementationType is not null));

        Assert.Contains(
collection: services,
filter: descriptor => descriptor.ServiceType == typeof(IWorkflowInstanceAggregationService)
                && descriptor.ImplementationType?.Name == "WorkflowInstanceAggregationService");
    }

    [Fact]
    public void AddWorkflowHostedServices_RegistersWorkflowEventHandlerPayloadTypes()
    {
        // Given
        IServiceCollection services = new ServiceCollection();
        services.AddLogging();
        services.AddEventing();
        services.AddWorkflowHostedServices();

        // When
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        IEventHub eventHub = serviceProvider.GetRequiredService<IEventHub>();

        // Then
        eventHub.ListenToWorkflowEvents();
    }
}