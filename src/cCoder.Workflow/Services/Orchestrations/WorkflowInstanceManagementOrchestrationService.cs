// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
            await RunInstanceMaintenanceAsync(cancellationToken: cancellationToken);
            await RunQueueInstanceManagementAsync(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            log.LogError(exception: ex, message: ex.Message);

            if (ex.InnerException != null)
            {
                log.LogError(exception: ex.InnerException, message: ex.InnerException.Message);
            }
        }
    }

    public object[] GetStats()
        => workflowInstanceManagementBroker.GetFailedExecutionStats();

    public async Task RunInstanceMaintenanceContinuouslyAsync(CancellationToken cancellationToken = default)
    {
        if (workflowConfiguration.IsMigrating)
        {
            return;
        }

        await RunInstanceMaintenanceAsync(cancellationToken: cancellationToken);

        using PeriodicTimer timer = new(TimeSpan.FromMinutes(minutes: 1));

        while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken: cancellationToken))
        {
            await RunInstanceMaintenanceAsync(cancellationToken: cancellationToken);
        }
    }

    public async Task RunInstanceMaintenanceAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await DropOldInstancesAsync(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            log.LogError(exception: ex, message: ex.Message);

            if (ex.InnerException != null)
            {
                log.LogError(exception: ex.InnerException, message: ex.InnerException.Message);
            }
        }
    }

    public async Task RunQueueInstanceManagementContinuouslyAsync(CancellationToken cancellationToken = default)
    {
        if (workflowConfiguration.IsMigrating)
        {
            return;
        }

        await RunQueueInstanceManagementAsync(cancellationToken: cancellationToken);

        using PeriodicTimer timer = new(TimeSpan.FromMinutes(minutes: 1));

        while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken: cancellationToken))
        {
            await RunQueueInstanceManagementAsync(cancellationToken: cancellationToken);
        }
    }

    public async Task RunQueueInstanceManagementAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await ExecuteQueuedInstancesAsync(cancellationToken: cancellationToken);
            await RequeueHungExecutingInstancesAsync(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            log.LogError(exception: ex, message: ex.Message);

            if (ex.InnerException != null)
            {
                log.LogError(exception: ex.InnerException, message: ex.InnerException.Message);
            }
        }
    }

    private async ValueTask ExecuteQueuedInstancesAsync(CancellationToken cancellationToken)
    {
        FlowInstanceData[] queuedInstances = workflowInstanceManagementBroker.GetQueuedInstances();

        foreach (FlowInstanceData queuedInstance in queuedInstances)
        {
            await ExecuteInstanceAsync(instanceId: queuedInstance.Id, cancellationToken: cancellationToken);
        }
    }

    public async ValueTask ExecuteWaitingQueuedInstanceByIdAsync(Guid id)
    {
        await ExecuteInstanceAsync(instanceId: id);
    }

    private async ValueTask DropOldInstancesAsync(CancellationToken cancellationToken)
    {
        int dropCount = await workflowInstanceManagementBroker
            .FlushOldInstancesAsync(cutoff: DateTimeOffset.UtcNow.Subtract(value: GetInstanceMaintenanceMaxAge()), cancellationToken: cancellationToken);

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
            .RequeueHungExecutingInstancesAsync(cutoff: DateTimeOffset.UtcNow.Subtract(value: GetExecutingInstanceTimeout()), cancellationToken: cancellationToken);

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
            .ClaimQueuedInstanceAsync(id: instanceId, cancellationToken: cancellationToken);

        if (dbInstance == null)
        {
            return;
        }

        try
        {
            ITokenManager tokenManager = serviceProvider.GetRequiredService<ITokenManager>();
            Token token = await tokenManager.IssueTokenAsync(userId: dbInstance.Caller, tokenUse: TokenUse.WorkflowExecution);

            WorkflowRequest request = CreateWorkflowRequest(dbInstance: dbInstance, token: token);

            HttpResponseMessage result = await SendToWorkflowAsync(request: request);

            if (!result.IsSuccessStatusCode)
            {
                string errorDetails = await result.Content.ReadAsStringAsync();

                log.LogError(
                    "Flow instance {InstanceId} execution failed.\n{ErrorDetails}",
                    dbInstance.Id,
                    errorDetails);

                await workflowInstanceManagementBroker.MarkInstanceFailedAsync(
id: dbInstance.Id,
failedAt: DateTimeOffset.UtcNow,
cancellationToken: cancellationToken);
            }
        }
        catch (Exception exception)
        {
            log.LogError(exception: exception, message: "Flow instance {InstanceId} execution failed.", args: dbInstance.Id);

            await workflowInstanceManagementBroker.MarkInstanceFailedAsync(
id: dbInstance.Id,
failedAt: DateTimeOffset.UtcNow,
cancellationToken: cancellationToken);
        }
    }

    private async ValueTask<HttpResponseMessage> SendToWorkflowAsync(WorkflowRequest request)
    {
        using HttpClient api = new(Handler)
        {
            BaseAddress = new Uri(appConfiguration["Services:Workflow"])
        };

        return await api.PostAsync(
requestUri: "Execute",
content: new StringContent(JsonSerializer.Serialize(value: request), System.Text.Encoding.UTF8, "application/json"));
    }

    internal WorkflowRequest CreateWorkflowRequest(FlowInstanceData dbInstance, Token token) =>
        new()
        {
            Api = $"https://{dbInstance.FlowDefinition.App.Domain}:{appConfiguration["Settings:sslPort"] ?? "443"}/Api/",
            FlowId = dbInstance.FlowDefinition.Id,
            AuthToken = token.Id,
            InstanceId = dbInstance.Id
        };

    private TimeSpan GetInstanceMaintenanceMaxAge() =>
        TimeSpan.FromDays(value: appConfiguration.GetValue<double?>(key: "Workflow:InstanceMaintenance:MaxAgeDays") ?? 7);

    private TimeSpan GetExecutingInstanceTimeout() =>
        TimeSpan.FromMinutes(value: appConfiguration.GetValue<double?>(key: "Workflow:QueueInstanceManagement:ExecutingTimeoutMinutes") ?? 30);
}