// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Workflow.Web.Models;

namespace Workflow.Web.Extensions;

internal static class IConfigurationExtensions
{
    internal static AppConfiguration CreateAppConfiguration(
        this IConfiguration configuration) =>
        new()
        {
            CoreData = new(),
            Eventing = new(),
            Security = new(),
            SecurityData = new(),
            Workflow = new()
            {
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