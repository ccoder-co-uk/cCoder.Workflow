// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Workflow;
using cCoder.Eventing;
using cCoder.Eventing.Models;
using cCoder.Workflow;
using cCoder.Workflow.Services.Processings;
using Workflow.HostedServices.Models;

namespace Workflow.HostedServices;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);

        builder.Services.AddWorkflowHostedServices(
            configuration: builder.Configuration,
            configure: configuration =>
                configuration.Eventing.EventProviders =
                    CreateWorkflowHostedServiceEventProviders());

        WebApplication app = builder.Build();
        app.MapControllers();
        app.StartWorkflowHostedServices();
        app.Run();
    }

    private static EventProvider[] CreateWorkflowHostedServiceEventProviders() =>
    [
        CreateAppEventProvider(),
        CreateQueuedFlowInstanceDataEventProvider()
    ];

    private static EventProvider<App> CreateAppEventProvider() =>
        new()
        {
            Events = ["app_add", "app_update", "app_delete"],
            ReceiveHandler = async (serviceProvider, eventName, message) =>
            {
                IEventHub eventHub =
                    serviceProvider.GetRequiredService<IEventHub>();

                EventMessage<App> eventMessage = new()
                {
                    AuthInfo = new EventAuthInfo
                    {
                        SSOUserId = message.AuthInfo?.SSOUserId ?? "Guest"
                    },
                    Data = message.Data
                };

                await eventHub.RaiseEventAsync(
                    name: eventName,
                    message: eventMessage);
            }
        };

    private static EventProvider<FlowInstanceData>
        CreateQueuedFlowInstanceDataEventProvider() =>
        new()
        {
            Events = ["flow_instance_data_add"],
            ReceiveHandler = async (serviceProvider, _, message) =>
            {
                if (message.Data?.Id == Guid.Empty)
                {
                    throw new InvalidOperationException(
                        message:
                            "A queued workflow instance requires a valid id.");
                }

                if (!string.Equals(
                    a: message.Data?.State,
                    b: "Queued",
                    comparisonType: StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                IWorkflowInstanceProcessingService processingService =
                    serviceProvider.GetRequiredService<
                        IWorkflowInstanceProcessingService>();

                await processingService
                    .ExecuteWaitingQueuedInstanceByIdAsync(
                        flowInstanceDataId: message.Data.Id);
            }
        };
}