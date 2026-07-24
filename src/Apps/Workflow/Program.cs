// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using cCoder.Workflow.Engine;

IHost host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureLogging(configureLogging:loggingBuilder =>
    {
        IConfigurationRoot configRoot = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("host.json", optional: false, reloadOnChange: true)
            .Build();

        loggingBuilder.ClearProviders();
        loggingBuilder.AddSimpleConsole(options => options.SingleLine = true);
        loggingBuilder.AddFilter(level => level >= LogLevel.Debug);
        loggingBuilder.AddConfiguration(configRoot.GetSection("logging"));
    })
    .ConfigureServices(configureDelegate:services =>
    {
        services.AddWorkflowEngine();
    })
    .Build();

await host.RunAsync();