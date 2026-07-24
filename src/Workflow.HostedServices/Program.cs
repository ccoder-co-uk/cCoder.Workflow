// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Workflow.HostedServices.Extensions;

namespace Workflow.HostedServices;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);

        builder.Services.AddWorkflowHostedServicesApplication(
            configurationManager: builder.Configuration,
            environment: builder.Environment);

        WebApplication app = builder.Build();
        app.UseWorkflowHostedServicesApplication();
        app.Run();
    }
}