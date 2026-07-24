// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.Data;
using cCoder.Data.Extensions;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Web.AcceptanceTests.Infrastructure;
using Xunit;
using SsoToken = cCoder.Security.Objects.Entities.Token;

namespace Web.AcceptanceTests.Tests.Integration;

[Collection(IntegrationAcceptanceCollection.Name)]
public sealed class WorkflowExecutionIntegrationTests(IntegrationAcceptanceFixture fixture)
{
    private const string AdminUserId = "admin";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    private const string SimpleFlowDefinitionJson =
        "{\"Name\":\"Acceptance\",\"Activities\":[{\"$type\":\"cCoder.Workflow.Activities.Start, cCoder.Workflow.Activities\",\"Ref\":\"start\"}],\"Links\":[]}";

    [Fact]
    public async Task ManualFlowExecution_ShouldQueueThroughHostedServicesAndCompleteInWorkflowApp()
    {
        (int appId, Guid flowId, Guid roleId) = await CreateAppWithExecutableFlowAsync();

        try
        {
            string authToken = await CreateAuthTokenAsync(userId: AdminUserId);

            await PostRawAsync(relativeUrl: $"/Api/Workflow/FlowDefinition({flowId})/Execute", body: "{}", authToken: authToken);

            await WaitUntilAsync(predicate: async () => await HasFlowInstanceStateAsync(flowId: flowId, state: "Complete"),
                diagnosticsFactory: () => BuildFlowDiagnosticsAsync(flowId: flowId));

            FlowInstanceData instance = await GetLatestInstanceAsync(flowId: flowId);
            instance.Caller.Should()
                .Be(expected: AdminUserId);
            instance.State.Should()
                .Be(expected: "Complete");
            instance.ContextString.Should()
                .Contain(expected: "Execution complete.");
            instance.ContextString.Should()
                .NotContain(unexpected: "Execution failed.");
        }
        finally
        {
            await DeleteAppGraphAsync(appId: appId, roleId: roleId);
        }
    }

    private async Task<(int appId, Guid flowId, Guid roleId)> CreateAppWithExecutableFlowAsync()
    {
        await using CoreDataContext core = CreateCoreContext();
        string unique = Guid.NewGuid()
            .ToString(format: "N");
        Guid roleId = Guid.NewGuid();

        App app = await core.AddAppAsync(app: new App
        {
            Name = $"Workflow Integration {unique}",
            Domain = "localhost",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = $"tenant-{unique}",
            ConfigJson = "{}",
            Roles =
            [
                new Role
                {
                    Id = roleId,
                    Name = $"Workflow Execute {unique}",
                    Description = "Workflow integration role",
                    Privs = "app_admin,flowdefinition_create,flowdefinition_read,flowdefinition_update,flowdefinition_delete,flowdefinition_execute,flowinstancedata_read,flowinstancedata_update",
                    Users = [new UserRole { RoleId = roleId, UserId = AdminUserId }]
                }
            ]
        });

        using IServiceScope scope = fixture.DatabaseServices.CreateScope();
        IAppOrchestrationService appSecurity = scope.ServiceProvider.GetRequiredService<IAppOrchestrationService>();
        await appSecurity.AddAppAsync(app: app);

        FlowDefinition flow = await core.AddFlowDefinitionAsync(flowDefinition: new FlowDefinition
        {
            AppId = app.Id,
            Name = $"Manual Flow {unique}",
            Description = "Integration flow",
            DefinitionJson = SimpleFlowDefinitionJson,
            ConfigJson = "{}",
            CreatedBy = AdminUserId,
            CreatedOn = DateTimeOffset.UtcNow,
            LastUpdatedBy = AdminUserId,
            LastUpdated = DateTimeOffset.UtcNow
        });

        return (app.Id, flow.Id, roleId);
    }

    private async Task PostRawAsync(string relativeUrl, string body, string authToken = null)
    {
        using HttpRequestMessage request = new(HttpMethod.Post, relativeUrl)
        {
            Content = new StringContent(body ?? string.Empty, Encoding.UTF8, "application/json")
        };

        if (!string.IsNullOrWhiteSpace(value: authToken))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
        }

        using HttpResponseMessage response = await fixture.WebClient.SendAsync(request: request);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should()
            .Be(
expected: HttpStatusCode.OK,
because: "{0}",
becauseArgs: content + Environment.NewLine + Environment.NewLine + await BuildFlowDiagnosticsAsync(flowId: Guid.Empty));
    }

    private async Task<string> CreateAuthTokenAsync(string userId)
    {
        await using DbContext sso = fixture.DatabaseServices
            .GetRequiredService<ISecurityDbContextFactory>()
            .CreateDbContext(ignoreAuthInfo: true);

        string tokenId = Guid.NewGuid()
            .ToString(format: "N");

        sso.Add(entity: new SsoToken
        {
            Id = tokenId,
            Reason = (int)TokenUse.Auth,
            Expires = DateTimeOffset.UtcNow.AddHours(hours: 1),
            UserName = userId
        });

        await sso.SaveChangesAsync();
        return tokenId;
    }

    private async Task<bool> HasFlowInstanceStateAsync(Guid flowId, string state)
    {
        await using CoreDataContext core = CreateCoreContext();
        return await core.Set<FlowInstanceData>()
            .IgnoreQueryFilters()
            .AnyAsync(predicate: instance => instance.FlowDefinitionId == flowId && instance.State == state);
    }

    private async Task<FlowInstanceData> GetLatestInstanceAsync(Guid flowId)
    {
        await using CoreDataContext core = CreateCoreContext();
        return await core.Set<FlowInstanceData>()
            .IgnoreQueryFilters()
            .Where(predicate: instance => flowId == Guid.Empty || instance.FlowDefinitionId == flowId)
            .OrderByDescending(keySelector: instance => instance.Start)
            .FirstAsync();
    }

    private async Task DeleteAppGraphAsync(int appId, Guid roleId)
    {
        await using CoreDataContext core = CreateCoreContext();

        FlowInstanceData[] instances = await core.Set<FlowInstanceData>()
            .IgnoreQueryFilters()
            .Where(predicate: instance => instance.FlowDefinition.AppId == appId)
            .ToArrayAsync();
        await core.DeleteAllAsync(flowInstances: instances);

        FlowDefinition[] flows = await core.Set<FlowDefinition>()
            .IgnoreQueryFilters()
            .Where(predicate: flow => flow.AppId == appId)
            .ToArrayAsync();
        await core.DeleteAllAsync(flowDefinitions: flows);

        Guid[] roleIds = await core.Set<Role>()
            .IgnoreQueryFilters()
            .Where(predicate: role => role.AppId == appId || role.Id == roleId)
            .Select(selector: role => role.Id)
            .ToArrayAsync();

        UserRole[] userRoles = await core.Set<UserRole>()
            .IgnoreQueryFilters()
            .Where(predicate: userRole => roleIds.Contains(value: userRole.RoleId))
            .ToArrayAsync();
        await core.DeleteAllAsync(userRoles: userRoles);

        Role[] roles = await core.Set<Role>()
            .IgnoreQueryFilters()
            .Where(predicate: role => roleIds.Contains(value: role.Id))
            .ToArrayAsync();
        await core.DeleteAllAsync(roles: roles);

        App app = await core.Set<App>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(predicate: found => found.Id == appId);

        if (app is not null)
        {
            await core.DeleteAsync(app: app);
        }
    }

    private async Task<string> BuildFlowDiagnosticsAsync(Guid flowId)
    {
        await using CoreDataContext core = CreateCoreContext();

        FlowInstanceData[] instances = await core.Set<FlowInstanceData>()
            .IgnoreQueryFilters()
            .Where(predicate: instance => instance.FlowDefinitionId == flowId)
            .OrderByDescending(keySelector: instance => instance.Start)
            .ToArrayAsync();

        string instanceSummary = instances.Length == 0
            ? "No flow instances were found."
            : string.Join(
separator: Environment.NewLine,
values: instances.Select(selector: instance =>
                    $"Instance {instance.Id} | State={instance.State} | Start={instance.Start:u} | End={(instance.End.HasValue ? instance.End.Value.ToString("u") : "<null>")} | Context={instance.ContextString ?? "<null>"}"));

        return string.Join(
separator: Environment.NewLine + Environment.NewLine,
value: [
                "Flow instances:",
                instanceSummary,
                "HostedServices output:",
                TakeLastLines(content:fixture.HostedServicesOutput, maxLines:200),
                "Workflow output:",
                TakeLastLines(content:fixture.WorkflowOutput, maxLines:200),
                "Web output:",
                TakeLastLines(content:fixture.WebOutput, maxLines:200)
            ]);
    }

    private static async Task WaitUntilAsync(
        Func<Task<bool>> predicate,
        int attempts = 60,
        int delayMilliseconds = 500,
        Func<Task<string>> diagnosticsFactory = null)
    {
        for (int attempt = 0; attempt < attempts; attempt++)
        {
            if (await predicate())
            {
                return;
            }

            await Task.Delay(millisecondsDelay: delayMilliseconds);
        }

        string diagnostics = diagnosticsFactory is null
            ? string.Empty
            : $"{Environment.NewLine}{Environment.NewLine}{await diagnosticsFactory()}";

        throw new TimeoutException($"Timed out waiting for the expected condition.{diagnostics}");
    }

    private CoreDataContext CreateCoreContext() =>
        fixture.DatabaseServices.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();

    private static string TakeLastLines(string content, int maxLines)
    {
        if (string.IsNullOrWhiteSpace(value: content))
        {
            return "<no output>";
        }

        string[] lines = content
            .Split(separator: Environment.NewLine, options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return string.Join(separator: Environment.NewLine, values: lines.TakeLast(count: maxLines));
    }
}