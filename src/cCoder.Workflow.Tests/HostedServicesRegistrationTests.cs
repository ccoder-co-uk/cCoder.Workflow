using cCoder.Workflow.Exposures.HostedServices;
using cCoder.Workflow.Services.Orchestrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;


namespace cCoder.Workflow.Tests;

public class HostedServicesRegistrationTests
{
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
}
