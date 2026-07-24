// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;
using cCoder.Workflow.Exposures.EventHandlers;
using cCoder.Workflow.Dependencies.HostedServices;
using cCoder.Workflow.Services.Processings;
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
filter: descriptor => descriptor.ServiceType == typeof(IInstanceMaintenanceBackgroundServiceDependency)
                && descriptor.ImplementationType == typeof(InstanceMaintenanceBackgroundServiceDependency));

        Assert.Contains(
collection: services,
filter: descriptor => descriptor.ServiceType == typeof(IQueueInstanceBackgroundServiceDependency)
                && descriptor.ImplementationType == typeof(QueueInstanceBackgroundServiceDependency));

        Assert.Contains(
collection: services,
filter: descriptor => descriptor.ServiceType == typeof(IScheduledTaskRunnerBackgroundServiceDependency)
                && descriptor.ImplementationType == typeof(ScheduledTaskRunnerBackgroundServiceDependency));

        Assert.Equal(
expected: 3,
actual: services.Count(predicate: descriptor => descriptor.ServiceType == typeof(IHostedService)
                && descriptor.ImplementationFactory is not null));

        Assert.Contains(
collection: services,
filter: descriptor => descriptor.ServiceType == typeof(IWorkflowInstanceProcessingService)
                && descriptor.ImplementationType?.Name == "WorkflowInstanceProcessingService");
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
        IWorkflowEventHandlers handlers = serviceProvider.GetRequiredService<IWorkflowEventHandlers>();

        // Then
        handlers.ListenToAllEvents();
        handlers.ListenToScheduledTaskExecuteEvents();
        handlers.ListenToQueuedFlowInstanceExecuteEvents();
    }
}