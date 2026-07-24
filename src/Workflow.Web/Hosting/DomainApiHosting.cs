// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Apps.Shared.Hosting;


namespace Apps.Shared;

public static class DomainApiHosting
{
    public static void UseDomainApiShell(this WebApplication app)
    {
        app.UseRouting();
        app.MapControllers();
    }

    public static void UseDomainDefaultCors(this WebApplication app)
    {
        app.UseCors(configurePolicy: builder =>
        {
            builder.AllowAnyHeader();
            builder.AllowAnyMethod();
            builder.SetIsOriginAllowed(isOriginAllowed: _ => true);
            builder.AllowCredentials();
        });
    }

    public static void UseDomainExceptionHandling(
        this WebApplication app,
        RequestDelegate errorHandler
    )
    {
        app.UseExceptionHandler(configure: errorApp => errorApp.Run(handler: errorHandler));
    }
}