using System.Net;
using System.Text.Json;
using cCoder.Data.Models.Workflow;
using cCoder.Security.Exposures;
using cCoder.Security.Objects.Entities;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Brokers;

namespace cCoder.Workflow.Services.Orchestrations;

internal sealed class WorkflowInstanceManagementOrchestrationService(
    IWorkflowInstanceManagementBroker workflowInstanceManagementBroker,
    IServiceProvider serviceProvider,
    IConfiguration configuration,
    ILogger<WorkflowInstanceManagementOrchestrationService> log)
    : IWorkflowInstanceManagementOrchestrationService
{
    private static readonly HttpClientHandler Handler = new()
    {
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
    };

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await DropOldInstancesAsync(cancellationToken);
            await RequeueHungExecutingInstancesAsync(cancellationToken);
            await ExecuteWaitingQueuedInstancesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);

            if (ex.InnerException != null)
                log.LogError(ex.InnerException, ex.InnerException.Message);
        }
    }

    public object[] GetStats()
        => workflowInstanceManagementBroker.GetFailedExecutionStats();

    public async ValueTask ExecuteWaitingQueuedInstanceByIdAsync(Guid id)
    {
        await ExecuteInstanceAsync(id);
    }

    private async ValueTask DropOldInstancesAsync(CancellationToken cancellationToken)
    {
        int dropCount = await workflowInstanceManagementBroker
            .FlushOldInstancesAsync(DateTimeOffset.UtcNow.AddDays(-7), cancellationToken);

        if (dropCount > 0)
            log.LogInformation("Dropped {Count} Workflow instances older than 7 days.", dropCount);
    }

    private async ValueTask RequeueHungExecutingInstancesAsync(CancellationToken cancellationToken)
    {
        int requeueCount = await workflowInstanceManagementBroker
            .RequeueHungExecutingInstancesAsync(DateTimeOffset.UtcNow.AddMinutes(-30), cancellationToken);

        if (requeueCount > 0)
        {
            log.LogWarning(
                "Requeued {Count} Workflow instances that were still executing after 30 minutes.",
                requeueCount);
        }
    }

    private async ValueTask ExecuteWaitingQueuedInstancesAsync(CancellationToken cancellationToken)
    {
        List<Task> executions = [];

        foreach (Guid instanceId in workflowInstanceManagementBroker.GetQueuedInstances()
            .Select(instance => instance.Id)
            .Distinct())
        {
            cancellationToken.ThrowIfCancellationRequested();
            executions.Add(ExecuteInstanceAsync(instanceId, cancellationToken));
        }

        await Task.WhenAll(executions);
    }

    private async Task ExecuteInstanceAsync(Guid instanceId, CancellationToken cancellationToken = default)
    {
        FlowInstanceData dbInstance = await workflowInstanceManagementBroker
            .ClaimQueuedInstanceAsync(instanceId, cancellationToken);

        if (dbInstance == null)
            return;

        IAccountManager accountManager = serviceProvider.GetRequiredService<IAccountManager>();
        Token token = await accountManager.IssueTokenAsync(dbInstance.Caller);

        WorkflowRequest request = new()
        {
            Api = $"https://{dbInstance.FlowDefinition.App.Domain}:{configuration["Settings:sslPort"] ?? "443"}/Api/",
            FlowId = dbInstance.FlowDefinition.Id,
            AuthToken = token.Id,
            InstanceId = dbInstance.Id
        };

        HttpResponseMessage result = await SendToWorkflowAsync(request);

        if (!result.IsSuccessStatusCode)
            log.LogError(
                "Flow instance {InstanceId} execution failed.\n{ErrorDetails}",
                dbInstance.Id,
                await result.Content.ReadAsStringAsync());
    }

    private async ValueTask<HttpResponseMessage> SendToWorkflowAsync(WorkflowRequest request)
    {
        using HttpClient api = new(Handler)
        {
            BaseAddress = new Uri(configuration["Services:Workflow"])
        };

        return await api.PostAsync(
            "Execute",
            new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json"));
    }
}
