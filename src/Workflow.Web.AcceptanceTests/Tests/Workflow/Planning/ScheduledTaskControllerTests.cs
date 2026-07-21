using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using cCoder.Data;
using cCoder.Data.Extensions;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Web.AcceptanceTests.Infrastructure;
using Xunit;


using Microsoft.EntityFrameworkCore;
namespace Web.AcceptanceTests.Tests.Workflow.Planning;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class ScheduledTaskControllerTests(WebAcceptanceFixture fixture)
{
    private const int AppId = 1;
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Workflow/ScheduledTask";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededScheduledTaskContext(int AppId, Guid RoleId, Guid FlowId);
    private sealed record ODataEnvelope<T>(List<T> Value);

    private async Task<SeededScheduledTaskContext> SeedDatabase()
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        Role role = await core.AddRoleAsync(new Role
        {
            Id = Guid.NewGuid(),
            AppId = AppId,
            Name = Unique("AcceptanceRole"),
            Description = "Acceptance role",
            Privs = "app_admin,flowdefinition_read,flowdefinition_execute,scheduledtask_create,scheduledtask_update,scheduledtask_delete,scheduledtask_read",
        });

        await core.AddUserRoleAsync(new UserRole { RoleId = role.Id, UserId = "Guest" });

        FlowDefinition flowDefinition = await core.AddFlowDefinitionAsync(new FlowDefinition
        {
            AppId = AppId,
            Name = Unique("Flow"),
            Description = "Acceptance flow",
            DefinitionJson =
                "{\"Name\":\"Acceptance\",\"Activities\":[{\"$type\":\"cCoder.Workflow.Activities.Start, cCoder.Workflow.Activities\",\"Ref\":\"start\"}],\"Links\":[]}",
            ConfigJson = "{}",
        });

        return new SeededScheduledTaskContext(AppId, role.Id, flowDefinition.Id);
    }

    private async Task<ScheduledTask> CreateScheduledTaskAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(BaseUrl, payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<ScheduledTask>(content, JsonOptions)
            ?? throw new InvalidOperationException("Expected scheduled task payload.");
    }

    private async Task<int> UpdateScheduledTaskAsync(int id, object payload)
    {
        using HttpResponseMessage response = await Client.PutAsJsonAsync($"{BaseUrl}({id})", payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<int> PatchScheduledTaskAsync(int id, object payload)
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

    private async Task<int> DeleteScheduledTaskAsync(int id)
    {
        using HttpResponseMessage response = await Client.DeleteAsync($"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<ScheduledTask> GetScheduledTaskAsync(int id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<ScheduledTask>(content, JsonOptions)
            ?? throw new InvalidOperationException("Expected scheduled task payload.");
    }

    private async Task Teardown(SeededScheduledTaskContext seededContext)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        FlowInstanceData[] instances = core.Set<FlowInstanceData>().IgnoreQueryFilters().Where(instance => instance.FlowDefinitionId == seededContext.FlowId).ToArray();
        await core.DeleteAllAsync(instances);

        ScheduledTask[] tasks = core.Set<ScheduledTask>().IgnoreQueryFilters().Where(task => task.AppId == seededContext.AppId).ToArray();
        await core.DeleteAllAsync(tasks);

        FlowDefinition flow = core.Set<FlowDefinition>().IgnoreQueryFilters().Single(found => found.Id == seededContext.FlowId);
        await core.DeleteAsync(flow);

        UserRole[] userRoles = core.Set<UserRole>().IgnoreQueryFilters().Where(userRole => userRole.RoleId == seededContext.RoleId).ToArray();
        await core.DeleteAllAsync(userRoles);

        Role role = core.Set<Role>().IgnoreQueryFilters().Single(found => found.Id == seededContext.RoleId);
        await core.DeleteAsync(role);

    }

    private async Task<int> GetScheduledTaskCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return int.Parse(content);
    }

    private async Task<IReadOnlyList<ScheduledTask>> GetScheduledTasksAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<ODataEnvelope<ScheduledTask>>(content, JsonOptions)?.Value
            ?? throw new InvalidOperationException("Expected scheduled task OData payload.");
    }

    private async Task<int> ExecuteScheduledTaskAsync(int id, bool incrementNextExecution)
    {
        using HttpResponseMessage response = await Client.PostAsync($"{BaseUrl}({id})/Execute?incrementNextExecution={incrementNextExecution.ToString().ToLowerInvariant()}", content: null);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<bool> HasQueuedInstanceAsync(Guid flowId)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();
        return await Task.FromResult(core.Set<FlowInstanceData>().IgnoreQueryFilters().Any(instance => instance.FlowDefinitionId == flowId));
    }
    private async Task<int> GetScheduledTaskStatusCodeAsync(int id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}({id})");
        return (int)response.StatusCode;
    }
}








