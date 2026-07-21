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
            string authToken = await CreateAuthTokenAsync(AdminUserId);

            await PostRawAsync($"/Api/Workflow/FlowDefinition({flowId})/Execute", "{}", authToken);

            await WaitUntilAsync(async () => await HasFlowInstanceStateAsync(flowId, "Complete"),
                diagnosticsFactory: () => BuildFlowDiagnosticsAsync(flowId));

            FlowInstanceData instance = await GetLatestInstanceAsync(flowId);
            instance.Caller.Should().Be(AdminUserId);
            instance.State.Should().Be("Complete");
            instance.ContextString.Should().Contain("Execution complete.");
            instance.ContextString.Should().NotContain("Execution failed.");
        }
        finally
        {
            await DeleteAppGraphAsync(appId, roleId);
        }
    }

    private async Task<(int appId, Guid flowId, Guid roleId)> CreateAppWithExecutableFlowAsync()
    {
        await using CoreDataContext core = CreateCoreContext();
        string unique = Guid.NewGuid().ToString("N");
        Guid roleId = Guid.NewGuid();

        App app = await core.AddAppAsync(new App
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
        await appSecurity.AddAsync(app);

        FlowDefinition flow = await core.AddFlowDefinitionAsync(new FlowDefinition
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

        if (!string.IsNullOrWhiteSpace(authToken))
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

        using HttpResponseMessage response = await fixture.WebClient.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(
            HttpStatusCode.OK,
            "{0}",
            content + Environment.NewLine + Environment.NewLine + await BuildFlowDiagnosticsAsync(Guid.Empty));
    }

    private async Task<string> CreateAuthTokenAsync(string userId)
    {
        await using DbContext sso = fixture.DatabaseServices
            .GetRequiredService<ISecurityDbContextFactory>()
            .CreateDbContext(true);

        string tokenId = Guid.NewGuid().ToString("N");

        sso.Add(new SsoToken
        {
            Id = tokenId,
            Reason = (int)TokenUse.Auth,
            Expires = DateTimeOffset.UtcNow.AddHours(1),
            UserName = userId
        });

        await sso.SaveChangesAsync();
        return tokenId;
    }

    private async Task<bool> HasFlowInstanceStateAsync(Guid flowId, string state)
    {
        await using CoreDataContext core = CreateCoreContext();
        return await core.Set<FlowInstanceData>().IgnoreQueryFilters()
            .AnyAsync(instance => instance.FlowDefinitionId == flowId && instance.State == state);
    }

    private async Task<FlowInstanceData> GetLatestInstanceAsync(Guid flowId)
    {
        await using CoreDataContext core = CreateCoreContext();
        return await core.Set<FlowInstanceData>().IgnoreQueryFilters()
            .Where(instance => flowId == Guid.Empty || instance.FlowDefinitionId == flowId)
            .OrderByDescending(instance => instance.Start)
            .FirstAsync();
    }

    private async Task DeleteAppGraphAsync(int appId, Guid roleId)
    {
        await using CoreDataContext core = CreateCoreContext();

        FlowInstanceData[] instances = await core.Set<FlowInstanceData>().IgnoreQueryFilters()
            .Where(instance => instance.FlowDefinition.AppId == appId)
            .ToArrayAsync();
        await core.DeleteAllAsync(instances);

        FlowDefinition[] flows = await core.Set<FlowDefinition>().IgnoreQueryFilters()
            .Where(flow => flow.AppId == appId)
            .ToArrayAsync();
        await core.DeleteAllAsync(flows);

        Guid[] roleIds = await core.Set<Role>().IgnoreQueryFilters()
            .Where(role => role.AppId == appId || role.Id == roleId)
            .Select(role => role.Id)
            .ToArrayAsync();

        UserRole[] userRoles = await core.Set<UserRole>().IgnoreQueryFilters()
            .Where(userRole => roleIds.Contains(userRole.RoleId))
            .ToArrayAsync();
        await core.DeleteAllAsync(userRoles);

        Role[] roles = await core.Set<Role>().IgnoreQueryFilters()
            .Where(role => roleIds.Contains(role.Id))
            .ToArrayAsync();
        await core.DeleteAllAsync(roles);

        App app = await core.Set<App>().IgnoreQueryFilters()
            .FirstOrDefaultAsync(found => found.Id == appId);

        if (app is not null)
            await core.DeleteAsync(app);
    }

    private async Task<string> BuildFlowDiagnosticsAsync(Guid flowId)
    {
        await using CoreDataContext core = CreateCoreContext();

        FlowInstanceData[] instances = await core.Set<FlowInstanceData>().IgnoreQueryFilters()
            .Where(instance => instance.FlowDefinitionId == flowId)
            .OrderByDescending(instance => instance.Start)
            .ToArrayAsync();

        string instanceSummary = instances.Length == 0
            ? "No flow instances were found."
            : string.Join(
                Environment.NewLine,
                instances.Select(instance =>
                    $"Instance {instance.Id} | State={instance.State} | Start={instance.Start:u} | End={(instance.End.HasValue ? instance.End.Value.ToString("u") : "<null>")} | Context={instance.ContextString ?? "<null>"}"));

        return string.Join(
            Environment.NewLine + Environment.NewLine,
            [
                "Flow instances:",
                instanceSummary,
                "HostedServices output:",
                TakeLastLines(fixture.HostedServicesOutput, 200),
                "Workflow output:",
                TakeLastLines(fixture.WorkflowOutput, 200),
                "Web output:",
                TakeLastLines(fixture.WebOutput, 200)
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
                return;

            await Task.Delay(delayMilliseconds);
        }

        string diagnostics = diagnosticsFactory is null
            ? string.Empty
            : $"{Environment.NewLine}{Environment.NewLine}{await diagnosticsFactory()}";

        throw new TimeoutException($"Timed out waiting for the expected condition.{diagnostics}");
    }

    private CoreDataContext CreateCoreContext() =>
        fixture.DatabaseServices.GetRequiredService<ICoreContextFactory>().CreateCoreContext();

    private static string TakeLastLines(string content, int maxLines)
    {
        if (string.IsNullOrWhiteSpace(content))
            return "<no output>";

        string[] lines = content
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return string.Join(Environment.NewLine, lines.TakeLast(maxLines));
    }
}
