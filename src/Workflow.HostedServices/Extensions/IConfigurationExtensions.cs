// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Workflow.HostedServices.Models;

namespace Workflow.HostedServices.Extensions;

internal static class IConfigurationExtensions
{
    internal static WorkflowHostedServicesConfiguration
        CreateWorkflowHostedServicesConfiguration(
            this IConfiguration configuration) =>
        new()
        {
            Data = new(),
            Eventing = new(),
            Security = new(),
            Workflow = new()
            {
                ConnectionString = string.Empty,
                RootPath = "Api/Workflow",
                ServiceUrl = "https://localhost:7100/",
                SslPort = 443,
                InstanceMaintenance = new() { MaxAgeDays = 7 },
                QueueInstanceManagement = new()
                {
                    ExecutingTimeoutMinutes = 30,
                    PollingIntervalMilliseconds = 60000
                }
            }
        };
}