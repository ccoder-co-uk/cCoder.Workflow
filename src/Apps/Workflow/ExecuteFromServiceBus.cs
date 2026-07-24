// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace Workflow;

public sealed class ExecuteFromServiceBus(ILogger<ExecuteFromServiceBus> logger)
{
    public Task RunAsync(
        // [ServiceBusTrigger("%WorkflowQueueName%", Connection = "ConnectionStrings:ServiceBus")]
        string message)
    {
        logger.LogInformation(
message:            "Service Bus workflow trigger is scaffolded but disabled. Uncomment the trigger attribute to enable direct queue execution.");

        return Task.CompletedTask;
    }
}