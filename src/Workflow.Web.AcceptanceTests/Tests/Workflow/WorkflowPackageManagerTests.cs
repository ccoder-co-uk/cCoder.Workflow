// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Exposures;
using cCoder.Workflow.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Web.AcceptanceTests.Tests.Workflow;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class WorkflowPackageManagerTests(WebAcceptanceFixture fixture)
{
    [Fact]
    public async Task ShouldExportAndImportWorkflowsAndScheduledTasksAsync()
    {
        // Given
        int sourceAppId;
        int targetAppId;
        Guid sourceFlowId;
        int sourceTaskId;
        Guid targetRoleId;

        string suffix = Guid.NewGuid()
            .ToString(format: "N");

        string flowName = $"Package flow {suffix}";
        string taskName = $"Package task {suffix}";

        using (IServiceScope scope = fixture.Factory.Services.CreateScope())
        {
            using var core = scope.ServiceProvider
                .GetRequiredService<cCoder.Data.ICoreContextFactory>()
                .CreateCoreContext();

            App sourceApp = await core.AddAppAsync(app: CreateApp(
                name: $"Package source {suffix}",
                domain: $"package-source-{suffix}.local"));

            App targetApp = await core.AddAppAsync(app: CreateApp(
                name: $"Package target {suffix}",
                domain: $"package-target-{suffix}.local"));

            sourceAppId = sourceApp.Id;
            targetAppId = targetApp.Id;

            Role targetRole = await core.AddRoleAsync(role: new Role
            {
                Id = Guid.NewGuid(),
                AppId = targetAppId,
                Name = $"Package administrator {suffix}",
                Description = "Package acceptance administrator",
                Privs = "app_admin",
            });

            targetRoleId = targetRole.Id;

            _ = await core.AddUserRoleAsync(userRole: new UserRole
            {
                RoleId = targetRoleId,
                UserId = "Guest",
            });

            FlowDefinition sourceFlow = await core.AddFlowDefinitionAsync(
                flowDefinition: new FlowDefinition
                {
                    AppId = sourceAppId,
                    Name = flowName,
                    Description = "Package acceptance flow",
                    ReportingComponentName = "Flow report",
                    InstanceReportingComponentName = "Instance report",
                    DefinitionJson = "{}",
                    ConfigJson = "{\"enabled\":true}",
                    LastUpdated = DateTimeOffset.UtcNow,
                });

            sourceFlowId = sourceFlow.Id;

            ScheduledTask sourceTask = await core.InsertScheduledTaskAsync(
                scheduledTask: new ScheduledTask
                {
                    AppId = sourceAppId,
                    FlowId = sourceFlowId,
                    Name = taskName,
                    Description = "Package acceptance task",
                    ExecuteAs = "Guest",
                    ExecutionArgs = "{\"source\":\"package\"}",
                    ScheduleInTicks = TimeSpan.FromHours(hours: 2).Ticks,
                    CreatedBy = "Guest",
                    UpdatedBy = "Guest",
                    Created = DateTimeOffset.UtcNow,
                    LastUpdated = DateTimeOffset.UtcNow,
                    NextExecution = DateTimeOffset.UtcNow.AddHours(hours: 2),
                });

            sourceTaskId = sourceTask.Id;
        }

        // When
        try
        {
            WorkflowPackage workflowsPackage;
            WorkflowPackage scheduledTasksPackage;

            using (IServiceScope scope = fixture.Factory.Services.CreateScope())
            {
                IWorkflowPackageManager packageManager = scope.ServiceProvider
                    .GetRequiredService<IWorkflowPackageManager>();

                workflowsPackage = packageManager.ExportPackage(
                    appId: sourceAppId,
                    packageName: "Workflows");

                scheduledTasksPackage = packageManager.ExportPackage(
                    appId: sourceAppId,
                    packageName: "ScheduledTasks");

                await packageManager.ImportPackageAsync(
                    appId: targetAppId,
                    workflowPackage: workflowsPackage);

                await packageManager.ImportPackageAsync(
                    appId: targetAppId,
                    workflowPackage: scheduledTasksPackage);
            }

            using (IServiceScope scope = fixture.Factory.Services.CreateScope())
            {
                using var core = scope.ServiceProvider
                    .GetRequiredService<cCoder.Data.ICoreContextFactory>()
                    .CreateCoreContext();

                FlowDefinition[] importedFlows = await core.FlowDefinitions
                    .IgnoreQueryFilters()
                    .Where(predicate: flow =>
                        flow.AppId == targetAppId
                        && flow.Name == flowName)
                    .ToArrayAsync();

                ScheduledTask[] importedTasks = await core.ScheduledTasks
                    .IgnoreQueryFilters()
                    .Include(navigationPropertyPath: task => task.Flow)
                    .Where(predicate: task =>
                        task.AppId == targetAppId
                        && task.Name == taskName)
                    .ToArrayAsync();

                // Then
                WorkflowPackageItem exportedWorkflowItem = workflowsPackage.Items.Single();
                WorkflowPackageItem exportedScheduledTaskItem = scheduledTasksPackage.Items.Single();

                using (new AssertionScope())
                {
                    exportedWorkflowItem.Type.Should()
                        .Be(expected: "Workflow/FlowDefinition");

                    exportedScheduledTaskItem.Type.Should()
                        .Be(expected: "Workflow/ScheduledTask");

                    importedFlows.Should()
                        .ContainSingle();

                    importedTasks.Should()
                        .ContainSingle();
                }

                FlowDefinition importedFlow = importedFlows.Single();
                ScheduledTask importedTask = importedTasks.Single();

                importedFlow.Id.Should()
                    .NotBe(unexpected: sourceFlowId);

                importedFlow.Description.Should()
                    .Be(expected: "Package acceptance flow");

                importedFlow.ReportingComponentName.Should()
                    .Be(expected: "Flow report");

                importedFlow.InstanceReportingComponentName.Should()
                    .Be(expected: "Instance report");

                importedFlow.DefinitionJson.Should()
                    .Be(expected: "{}");

                importedFlow.ConfigJson.Should()
                    .Be(expected: "{\"enabled\":true}");

                importedTask.Id.Should()
                    .NotBe(unexpected: sourceTaskId);

                importedTask.Description.Should()
                    .Be(expected: "Package acceptance task");

                importedTask.Flow.Name.Should()
                    .Be(expected: flowName);

                importedTask.ExecuteAs.Should()
                    .Be(expected: "Guest");

                importedTask.ExecutionArgs.Should()
                    .Be(expected: "{\"source\":\"package\"}");

                importedTask.ScheduleInTicks.Should()
                    .Be(expected: TimeSpan.FromHours(hours: 2)
                        .Ticks);
            }
        }
        finally
        {
            using IServiceScope scope = fixture.Factory.Services.CreateScope();

            using var core = scope.ServiceProvider
                .GetRequiredService<cCoder.Data.ICoreContextFactory>()
                .CreateCoreContext();

            ScheduledTask[] tasks = await core.ScheduledTasks
                .IgnoreQueryFilters()
                .Where(predicate: task =>
                    task.AppId == sourceAppId || task.AppId == targetAppId)
                .ToArrayAsync();

            await core.DeleteAllAsync(scheduledTasks: tasks);

            FlowDefinition[] flows = await core.FlowDefinitions
                .IgnoreQueryFilters()
                .Where(predicate: flow =>
                    flow.AppId == sourceAppId || flow.AppId == targetAppId)
                .ToArrayAsync();

            await core.DeleteAllAsync(flowDefinitions: flows);

            UserRole[] userRoles = await core.UserRoles
                .IgnoreQueryFilters()
                .Where(predicate: userRole => userRole.RoleId == targetRoleId)
                .ToArrayAsync();

            await core.DeleteAllAsync(userRoles: userRoles);

            Role targetRole = await core.Roles
                .IgnoreQueryFilters()
                .SingleAsync(predicate: role => role.Id == targetRoleId);

            await core.DeleteAsync(role: targetRole);

            App[] apps = await core.Apps
                .IgnoreQueryFilters()
                .Where(predicate: app =>
                    app.Id == sourceAppId || app.Id == targetAppId)
                .ToArrayAsync();

            foreach (App app in apps)
            {
                await core.DeleteAsync(app: app);
            }
        }
    }

    private static App CreateApp(string name, string domain) =>
        new()
        {
            Name = name,
            Domain = domain,
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = $"tenant-{Guid.NewGuid():N}",
            ConfigJson = "{}",
        };
}