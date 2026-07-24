// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace Workflow.HostedServices.Services.Processings;

internal sealed partial class HomeProcessingService(
    IConfiguration configuration)
    : IHomeProcessingService
{
    public string GetHome() =>
        TryCatch(operation: () =>
        {
            double instanceMaxAgeDays =
                configuration.GetValue<double?>(
                    key: "Workflow:InstanceMaintenance:MaxAgeDays") ?? 7;

            double executingTimeoutMinutes =
                configuration.GetValue<double?>(
                    key: "Workflow:QueueInstanceBackgroundServiceDependency:ExecutingTimeoutMinutes") ?? 30;

            double scheduledTaskIntervalMinutes = 1;

            string[] lines =
            [
                "Workflow Hosted Services",
                string.Empty,
                "Hosted services:",
                $"- InstanceMaintenanceBackgroundServiceDependency: deletes workflow instances older than {instanceMaxAgeDays} day(s).",
                $"- QueueInstanceBackgroundServiceDependency: resets executing workflow instances older than {executingTimeoutMinutes} minute(s) back to Queued.",
                $"- ScheduledTaskRunnerBackgroundServiceDependency: checks scheduled tasks every {scheduledTaskIntervalMinutes} minute(s) and raises scheduled_task_execute events for due tasks.",
                string.Empty,
                "Event listeners:",
                "- app_add, app_update, app_delete -> forwards app events to the workflow event hub.",
                "- flow_instance_data_add -> executes newly queued workflow instances by id.",
            ];

            return string.Join(
                separator: Environment.NewLine,
                value: lines);
        });
}