// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;

namespace Workflow.HostedServices.Controllers;

[ApiController]
public sealed class HomeController(IConfiguration configuration) : ControllerBase
{
    [HttpGet("/")]
    public IActionResult Get()
    {
        double instanceMaxAgeDays =
            configuration.GetValue<double?>(key: "Workflow:InstanceMaintenance:MaxAgeDays") ?? 7;

        double executingTimeoutMinutes =
            configuration.GetValue<double?>(key: "Workflow:QueueInstanceManagement:ExecutingTimeoutMinutes") ?? 30;

        double scheduledTaskIntervalMinutes = 1;

        string[] lines =
        [
            "Workflow Hosted Services",
            string.Empty,
            "Hosted services:",
            $"- InstanceMaintenanceManagement: deletes workflow instances older than {instanceMaxAgeDays} day(s).",
            $"- QueueInstanceManagement: resets executing workflow instances older than {executingTimeoutMinutes} minute(s) back to Queued.",
            $"- ScheduledTaskRunnerManagement: checks scheduled tasks every {scheduledTaskIntervalMinutes} minute(s) and raises scheduled_task_execute events for due tasks.",
            string.Empty,
            "Event listeners:",
            "- app_add, app_update, app_delete -> forwards app events to the workflow event hub.",
            "- flow_instance_data_add -> executes newly queued workflow instances by id.",
        ];

        return Content(content: string.Join(separator: Environment.NewLine, value: lines), contentType: "text/plain");
    }
}