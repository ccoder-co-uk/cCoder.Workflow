// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Workflow.Web.Extensions;

namespace Workflow.Web;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);

        builder.Services.AddWorkflowWebApplication(
            configurationManager: builder.Configuration,
            environment: builder.Environment);

        WebApplication app = builder.Build();
        app.UseWorkflowWebApplication();
        app.Run();
    }
}