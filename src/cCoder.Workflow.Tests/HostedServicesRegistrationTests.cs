using cCoder.Eventing;
using cCoder.Workflow.Exposures.EventHandlers;
using cCoder.Workflow.Exposures.HostedServices;
using cCoder.Workflow.Services.Orchestrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;


namespace cCoder.Workflow.Tests;

public class HostedServicesRegistrationTests
{
    [Fact]
    public void AddWorkflowWeb_DoesNotRegisterWorkflowInstanceManagementHostedService()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddWorkflowWeb();

        Assert.DoesNotContain(
            services,
            descriptor => descriptor.ServiceType == typeof(IHostedService)
                && descriptor.ImplementationType == typeof(WorkflowInstanceManagementHostedService));
    }

    [Fact]
    public void AddWorkflowHostedServices_RegistersWorkflowInstanceManagementHostedService()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddWorkflowHostedServices();

        Assert.Contains(
            services,
            descriptor => descriptor.ServiceType == typeof(IHostedService)
                && descriptor.ImplementationType == typeof(WorkflowInstanceManagementHostedService));
        Assert.Contains(
            services,
            descriptor => descriptor.ServiceType == typeof(IWorkflowInstanceManagementOrchestrationService)
                && descriptor.ImplementationType?.Name == "WorkflowInstanceManagementOrchestrationService");
    }

    [Fact]
    public void AddWorkflowHostedServices_RegistersWorkflowEventHandlerPayloadTypes()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddLogging();
        services.AddEventing();
        services.AddWorkflowHostedServices();

        IServiceProvider serviceProvider = services.BuildServiceProvider();
        IWorkflowEventHandlers handlers = serviceProvider.GetRequiredService<IWorkflowEventHandlers>();

        handlers.ListenToAllEvents();
        handlers.ListenToScheduledTaskExecuteEvents();
        handlers.ListenToQueuedFlowInstanceExecuteEvents();
    }
}
