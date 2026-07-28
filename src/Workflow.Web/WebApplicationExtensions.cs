// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Workflow;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.OData;

namespace Workflow.Web;

internal static class WebApplicationExtensions
{
    internal static WebApplication UseWorkflowWeb(
        this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseSession();

        app.UseSwagger()
            .UseSwaggerUI(setupAction: options =>
            {
                options.SwaggerEndpoint(
                    url: "/swagger/Workflow/swagger.json",
                    name: "Workflow API");
            })
            .UseODataBatching()
            .UseODataRouteDebug();

        app.UseRouting();
        app.MapControllers();
        app.MapGet(
            pattern: "/",
            handler: () => Results.Redirect(url: "/tools/index.html"));

        app.StartWorkflowWeb(
            log: app.Services.GetRequiredService<ILogger<Program>>());

        app.UseCors(configurePolicy: policy =>
        {
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
            policy.AllowAnyOrigin();
        });

        app.UseExceptionHandler(configure: errorApp =>
            errorApp.Run(handler: HandleUnhandledExceptionAsync));

        return app;
    }

    private static async Task HandleUnhandledExceptionAsync(
        HttpContext context)
    {
        Exception exception =
            context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

        context.Response.StatusCode =
            exception is SecurityException
                ? StatusCodes.Status401Unauthorized
                : StatusCodes.Status500InternalServerError;

        context.Response.ContentType = "application/json";

        if (exception is null)
        {
            return;
        }

        ILogger<Program> logger =
            context.RequestServices.GetRequiredService<ILogger<Program>>();

        logger.LogError(
            exception: exception,
            message: "Unhandled workflow request failure.");

        await context.Response.WriteAsJsonAsync(
            value: new { error = exception.Message });
    }
}