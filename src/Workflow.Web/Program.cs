// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Eventing.Http;
using cCoder.Eventing.Models;
using Workflow.Web.Models;

namespace Workflow.Web;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);

        builder.Services.AddWorkflowWeb(
            configuration: builder.Configuration,
            configure: configuration =>
                configuration.Eventing.EventProviders =
                    CreateFlowInstanceDataEventProviders(
                        configuration: configuration));

        WebApplication app = builder.Build();
        app.UseWorkflowWeb();
        app.Run();
    }

    private static EventProvider[] CreateFlowInstanceDataEventProviders(
        WorkflowWebConfiguration configuration)
    {
        if (!string.Equals(
            a: configuration.Eventing.ProviderType,
            b: "Http",
            comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            return [];
        }

        return
        [
            new EventProvider<FlowInstanceData>
            {
                Events = ["flow_instance_data_add"],
                SendHandler = async (serviceProvider, eventName, message) =>
                {
                    IHttpEventHub httpEventHub =
                        serviceProvider.GetRequiredService<IHttpEventHub>();

                    await httpEventHub.RaiseEventAsync(
                        name: eventName,
                        message: message);
                }
            }
        ];
    }
}