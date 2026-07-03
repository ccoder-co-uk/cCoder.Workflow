using System.Net;
using System.Text.Json;
using cCoder.Data.Models.Workflow;
using cCoder.Security.Exposures;
using cCoder.Security.Objects.Entities;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Models;

namespace cCoder.Workflow.Services.Orchestrations;

internal sealed class WorkflowInstanceManagementOrchestrationService(
    IWorkflowInstanceManagementBroker workflowInstanceManagementBroker,
    IServiceProvider serviceProvider,
    IConfiguration appConfiguration,
    WorkflowConfiguration workflowConfiguration,
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
            await RunInstanceMaintenanceAsync(cancellationToken);
            await RunQueueInstanceManagementAsync(cancellationToken);
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

    public async Task RunInstanceMaintenanceContinuouslyAsync(CancellationToken cancellationToken = default)
    {
        if (workflowConfiguration.IsMigrating)
            return;

        await RunInstanceMaintenanceAsync(cancellationToken);

        using PeriodicTimer timer = new(TimeSpan.FromMinutes(1));

        while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken))
            await RunInstanceMaintenanceAsync(cancellationToken);
    }

    public async Task RunInstanceMaintenanceAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await DropOldInstancesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);

            if (ex.InnerException != null)
                log.LogError(ex.InnerException, ex.InnerException.Message);
        }
    }

    public async Task RunQueueInstanceManagementContinuouslyAsync(CancellationToken cancellationToken = default)
    {
        if (workflowConfiguration.IsMigrating)
            return;

        await RunQueueInstanceManagementAsync(cancellationToken);

        using PeriodicTimer timer = new(TimeSpan.FromMinutes(1));

        while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken))
            await RunQueueInstanceManagementAsync(cancellationToken);
    }

    public async Task RunQueueInstanceManagementAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await RequeueHungExecutingInstancesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);

            if (ex.InnerException != null)
                log.LogError(ex.InnerException, ex.InnerException.Message);
        }
    }

    public async ValueTask ExecuteWaitingQueuedInstanceByIdAsync(Guid id)
    {
        await ExecuteInstanceAsync(id);
    }

    private async ValueTask DropOldInstancesAsync(CancellationToken cancellationToken)
    {
        int dropCount = await workflowInstanceManagementBroker
            .FlushOldInstancesAsync(DateTimeOffset.UtcNow.Subtract(GetInstanceMaintenanceMaxAge()), cancellationToken);

        if (dropCount > 0)
        {
            log.LogInformation(
                "Dropped {Count} Workflow instances older than {MaxAge}.",
                dropCount,
                GetInstanceMaintenanceMaxAge());
        }
    }

    private async ValueTask RequeueHungExecutingInstancesAsync(CancellationToken cancellationToken)
    {
        int requeueCount = await workflowInstanceManagementBroker
            .RequeueHungExecutingInstancesAsync(DateTimeOffset.UtcNow.Subtract(GetExecutingInstanceTimeout()), cancellationToken);

        if (requeueCount > 0)
        {
            log.LogWarning(
                "Requeued {Count} Workflow instances that were still executing after {Timeout}.",
                requeueCount,
                GetExecutingInstanceTimeout());
        }
    }

    private async Task ExecuteInstanceAsync(Guid instanceId, CancellationToken cancellationToken = default)
    {
        FlowInstanceData dbInstance = await workflowInstanceManagementBroker
            .ClaimQueuedInstanceAsync(instanceId, cancellationToken);

        if (dbInstance == null)
            return;

        ITokenManager tokenManager = serviceProvider.GetRequiredService<ITokenManager>();
        Token token = await tokenManager.IssueTokenAsync(dbInstance.Caller, TokenUse.WorkflowExecution);

        WorkflowRequest request = new()
        {
            Api = $"https://{dbInstance.FlowDefinition.App.Domain}:{appConfiguration["Settings:sslPort"] ?? "443"}/Api/",
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
            BaseAddress = new Uri(appConfiguration["Services:Workflow"])
        };

        return await api.PostAsync(
            "Execute",
            new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json"));
    }

    private TimeSpan GetInstanceMaintenanceMaxAge() =>
        TimeSpan.FromDays(appConfiguration.GetValue<double?>("Workflow:InstanceMaintenance:MaxAgeDays") ?? 7);

    private TimeSpan GetExecutingInstanceTimeout() =>
        TimeSpan.FromMinutes(appConfiguration.GetValue<double?>("Workflow:QueueInstanceManagement:ExecutingTimeoutMinutes") ?? 30);
}
