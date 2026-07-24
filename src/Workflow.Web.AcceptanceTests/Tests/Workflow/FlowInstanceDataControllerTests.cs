// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
public sealed partial class FlowInstanceDataControllerTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Workflow/FlowInstanceData";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededFlowInstanceDataContext(int AppId, Guid RoleId, Guid FlowId, Guid InstanceId);
    private sealed record ODataEnvelope<T>(List<T> Value);

    private async Task<SeededFlowInstanceDataContext> SeedDatabase(bool includeInstance = false, params string[] privileges)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        App app = await core.AddAppAsync(app:new App
        {
            Name = Unique("AcceptanceApp"),
            Domain = $"{Unique("workflow-instance")}.local",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = Unique("tenant"),
            ConfigJson = "{}",
        });

        Role role = await core.AddRoleAsync(role:new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique("AcceptanceRole"),
            Description = "Acceptance role",
            Privs = privileges.Length == 0
                ? "app_admin,flowdefinition_read,flowinstancedata_create,flowinstancedata_update,flowinstancedata_delete,flowinstancedata_read"
                : string.Join(',', privileges),
        });

        await core.AddUserRoleAsync(userRole:new UserRole { RoleId = role.Id, UserId = "Guest" });

        FlowDefinition flow = await core.AddFlowDefinitionAsync(flowDefinition:new FlowDefinition
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

        Guid instanceId = Guid.Empty;

        if (includeInstance)
        {
            FlowInstanceData instance = await core.AddFlowInstanceDataAsync(flowInstanceData:new FlowInstanceData
            {
                Id = Guid.NewGuid(),
                FlowDefinitionId = flow.Id,
                Name = Unique("Instance"),
                State = "Queued",
                Caller = "Guest",
                ContextString = "{}",
                Start = DateTimeOffset.UtcNow,
            });

            instanceId = instance.Id;
        }

        return new SeededFlowInstanceDataContext(app.Id, role.Id, flow.Id, instanceId);
    }

    private async Task<FlowInstanceData> CreateFlowInstanceDataAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(requestUri:BaseUrl, value:payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(expected:HttpStatusCode.OK, because:content);
        return JsonSerializer.Deserialize<FlowInstanceData>(json:content, options:JsonOptions)!;
    }

    private async Task<int> UpdateFlowInstanceDataAsync(Guid id, object payload)
    {
        using HttpResponseMessage response = await Client.PutAsJsonAsync(requestUri:$"{BaseUrl}({id})", value:payload);
        response.StatusCode.Should().Be(expected:HttpStatusCode.NoContent);
        return (int)response.StatusCode;
    }

    private async Task<int> PatchFlowInstanceDataAsync(Guid id, object payload)
    {
        using HttpRequestMessage request = new(HttpMethod.Patch, $"{BaseUrl}({id})")
        {
            Content = JsonContent.Create(inputValue:payload),
        };
        using HttpResponseMessage response = await Client.SendAsync(request:request);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(expected:HttpStatusCode.OK, because:content);
        return (int)response.StatusCode;
    }

    private async Task<int> DeleteFlowInstanceDataAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.DeleteAsync(requestUri:$"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(expected:HttpStatusCode.OK, because:content);
        return (int)response.StatusCode;
    }

    private async Task<FlowInstanceData> GetFlowInstanceDataAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri:$"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(expected:HttpStatusCode.OK, because:content);

        return JsonSerializer.Deserialize<FlowInstanceData>(json:content, options:JsonOptions)
            ?? throw new InvalidOperationException("Expected flow instance payload.");
    }

    private async Task Teardown(SeededFlowInstanceDataContext seededContext)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        FlowInstanceData[] instances = core.Set<FlowInstanceData>().IgnoreQueryFilters().Where(predicate:instance => instance.FlowDefinitionId == seededContext.FlowId).ToArray();
        await core.DeleteAllAsync(flowInstances:instances);

        FlowDefinition flow = core.Set<FlowDefinition>().IgnoreQueryFilters().Single(predicate:found => found.Id == seededContext.FlowId);
        await core.DeleteAsync(flowDefinition:flow);

        UserRole[] userRoles = core.Set<UserRole>().IgnoreQueryFilters().Where(predicate:userRole => userRole.RoleId == seededContext.RoleId).ToArray();
        await core.DeleteAllAsync(userRoles:userRoles);

        Role role = core.Set<Role>().IgnoreQueryFilters().Single(predicate:found => found.Id == seededContext.RoleId);
        await core.DeleteAsync(role:role);

        App app = core.Set<App>().IgnoreQueryFilters().Single(predicate:found => found.Id == seededContext.AppId);
        await core.DeleteAsync(app:app);

    }

    private async Task<int> GetFlowInstanceDataCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri:$"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(expected:HttpStatusCode.OK, because:content);
        return int.Parse(s:content);
    }

    private async Task<IReadOnlyList<FlowInstanceData>> GetFlowInstanceDataAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri:$"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(expected:HttpStatusCode.OK, because:content);
        return JsonSerializer.Deserialize<ODataEnvelope<FlowInstanceData>>(json:content, options:JsonOptions)!.Value;
    }
    private async Task<int> GetFlowInstanceDataStatusCodeAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri:$"{BaseUrl}({id})");
        return (int)response.StatusCode;
    }
}