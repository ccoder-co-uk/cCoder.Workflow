using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using cCoder.Data;
using cCoder.Data.Extensions;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities;
using cCoder.Workflow.Activities.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Web.AcceptanceTests.Infrastructure;
using Xunit;


using Microsoft.EntityFrameworkCore;
namespace Web.AcceptanceTests.Tests.Workflow;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class WorkflowEventControllerTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Core/WorkflowEvent";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededWorkflowEventContext(int AppId, Guid RoleId, Guid FlowId, Guid EventId);
    private sealed record ODataEnvelope<T>(List<T> Value);

    private async Task<SeededWorkflowEventContext> SeedDatabase(bool includeEvent = false, params string[] privileges)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        App app = await core.AddAppAsync(new App
        {
            Name = Unique("AcceptanceApp"),
            Domain = $"{Unique("workflow-event")}.local",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = Unique("tenant"),
            ConfigJson = "{}",
        });

        Role role = await core.AddRoleAsync(new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique("AcceptanceRole"),
            Description = "Acceptance role",
            Privs = privileges.Length == 0
                ? "app_admin,flowdefinition_read,workflowevent_create,workflowevent_update,workflowevent_delete,workflowevent_read"
                : string.Join(',', privileges),
        });

        await core.AddUserRoleAsync(new UserRole { RoleId = role.Id, UserId = "Guest" });

        FlowDefinition flow = await core.AddFlowDefinitionAsync(new FlowDefinition
        {
            AppId = app.Id,
            Name = Unique("Flow"),
            Description = "Acceptance flow",
            DefinitionJson = new Flow
            {
                Name = "Acceptance",
                Activities = [new Start { Ref = "start" }],
                Links = [],
            }.ToJson(),
            ConfigJson = "{}",
        });

        Guid eventId = Guid.Empty;

        if (includeEvent)
        {
            WorkflowEvent workflowEvent = await core.AddWorkflowEventAsync(new WorkflowEvent
            {
                FlowId = flow.Id,
                Type = "Acceptance",
                EventContext = "{}",
                CreatedBy = "Guest",
                CreatedOn = DateTimeOffset.UtcNow,
                ExecuteAs = "Guest",
            });

            eventId = workflowEvent.Id;
        }

        return new SeededWorkflowEventContext(app.Id, role.Id, flow.Id, eventId);
    }

    private async Task<WorkflowEvent> CreateWorkflowEventAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(BaseUrl, payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<WorkflowEvent>(content, JsonOptions)!;
    }

    private async Task<int> UpdateWorkflowEventAsync(Guid id, object payload)
    {
        using HttpResponseMessage response = await Client.PutAsJsonAsync($"{BaseUrl}({id})", payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<int> PatchWorkflowEventAsync(Guid id, object payload)
    {
        using HttpRequestMessage request = new(HttpMethod.Patch, $"{BaseUrl}({id})")
        {
            Content = JsonContent.Create(payload),
        };
        using HttpResponseMessage response = await Client.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<int> DeleteWorkflowEventAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.DeleteAsync($"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<WorkflowEvent> GetWorkflowEventAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        return JsonSerializer.Deserialize<WorkflowEvent>(content, JsonOptions)
            ?? throw new InvalidOperationException("Expected workflow event payload.");
    }

    private async Task Teardown(SeededWorkflowEventContext seededContext)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        WorkflowEvent[] workflowEvents = core.Set<WorkflowEvent>().IgnoreQueryFilters().Where(workflowEvent => workflowEvent.FlowId == seededContext.FlowId).ToArray();
        await core.DeleteAllAsync(workflowEvents);

        FlowDefinition flow = core.Set<FlowDefinition>().IgnoreQueryFilters().Single(found => found.Id == seededContext.FlowId);
        await core.DeleteAsync(flow);

        UserRole[] userRoles = core.Set<UserRole>().IgnoreQueryFilters().Where(userRole => userRole.RoleId == seededContext.RoleId).ToArray();
        await core.DeleteAllAsync(userRoles);

        Role role = core.Set<Role>().IgnoreQueryFilters().Single(found => found.Id == seededContext.RoleId);
        await core.DeleteAsync(role);

        App app = core.Set<App>().IgnoreQueryFilters().Single(found => found.Id == seededContext.AppId);
        await core.DeleteAsync(app);

    }

    private async Task<int> GetWorkflowEventCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return int.Parse(content);
    }

    private async Task<IReadOnlyList<WorkflowEvent>> GetWorkflowEventsAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<ODataEnvelope<WorkflowEvent>>(content, JsonOptions)!.Value;
    }
    private async Task<int> GetWorkflowEventStatusCodeAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}({id})");
        return (int)response.StatusCode;
    }
}








