// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Net.Http.Json;
using System.Text;
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
public sealed partial class FlowDefinitionControllerTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Workflow/FlowDefinition";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededFlowDefinitionContext(int AppId, Guid RoleId, Guid FlowId);
    private sealed record ODataEnvelope<T>(List<T> Value);

    private async Task<SeededFlowDefinitionContext> SeedDatabase(bool includeFlow = false, params string[] privileges)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        App app = await core.AddAppAsync(app: new App
        {
            Name = Unique(prefix: "AcceptanceApp"),
            Domain = $"{Unique(prefix: "workflow")}.local",
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = Unique(prefix: "tenant"),
            ConfigJson = "{}",
        });

        Role role = await core.AddRoleAsync(role: new Role
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = Unique(prefix: "AcceptanceRole"),
            Description = "Acceptance role",
            Privs = privileges.Length == 0
                ? "app_admin,flowdefinition_create,flowdefinition_update,flowdefinition_execute,flowdefinition_delete,flowdefinition_read,workflowevent_create,workflowevent_update,workflowevent_delete,workflowevent_read,flowinstancedata_create,flowinstancedata_update,flowinstancedata_delete,flowinstancedata_read"
                : string.Join(separator: ',', value: privileges),
        });

        await core.AddUserRoleAsync(userRole: new UserRole { RoleId = role.Id, UserId = "Guest" });

        Guid flowId = Guid.Empty;

        if (includeFlow)
        {
            FlowDefinition flow = await core.AddFlowDefinitionAsync(flowDefinition: new FlowDefinition
            {
                AppId = app.Id,
                Name = Unique(prefix: "Flow"),
                Description = "Acceptance flow",
                DefinitionJson = new Flow
                {
                    Name = "Acceptance",
                    Activities = [new Start { Ref = "start" }],
                    Links = [],
                }.ToJson(),
                ConfigJson = "{}",
            });

            flowId = flow.Id;
        }

        return new SeededFlowDefinitionContext(app.Id, role.Id, flowId);
    }

    private async Task<FlowDefinition> CreateFlowDefinitionAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(requestUri: BaseUrl, value: payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(expected: HttpStatusCode.OK, because: content);
        return JsonSerializer.Deserialize<FlowDefinition>(json: content, options: JsonOptions)!;
    }

    private async Task<int> UpdateFlowDefinitionAsync(Guid id, object payload)
    {
        using HttpResponseMessage response = await Client.PutAsJsonAsync(requestUri: $"{BaseUrl}({id})", value: payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(expected: HttpStatusCode.OK, because: content);
        return (int)response.StatusCode;
    }

    private async Task<int> PatchFlowDefinitionAsync(Guid id, object payload)
    {
        using HttpRequestMessage request = new(HttpMethod.Patch, $"{BaseUrl}({id})")
        {
            Content = JsonContent.Create(inputValue: payload),
        };
        using HttpResponseMessage response = await Client.SendAsync(request: request);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(expected: HttpStatusCode.OK, because: content);
        return (int)response.StatusCode;
    }

    private async Task<int> DeleteFlowDefinitionAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.DeleteAsync(requestUri: $"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(expected: HttpStatusCode.OK, because: content);
        return (int)response.StatusCode;
    }

    private async Task<FlowDefinition> GetFlowDefinitionAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<FlowDefinition>(json: content, options: JsonOptions)
            ?? throw new InvalidOperationException("Expected flow definition payload.");
    }

    private async Task Teardown(SeededFlowDefinitionContext seededContext)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        FlowInstanceData[] appInstances = core.Set<FlowInstanceData>().IgnoreQueryFilters()
            .Where(predicate: instance => instance.FlowDefinition.AppId == seededContext.AppId)
            .ToArray();
        await core.DeleteAllAsync(flowInstances: appInstances);

        FlowDefinition[] flows = core.Set<FlowDefinition>().IgnoreQueryFilters()
            .Where(predicate: flow => flow.AppId == seededContext.AppId)
            .ToArray();
        await core.DeleteAllAsync(flowDefinitions: flows);

        UserRole[] userRoles = core.Set<UserRole>().IgnoreQueryFilters().Where(predicate: userRole => userRole.RoleId == seededContext.RoleId).ToArray();
        await core.DeleteAllAsync(userRoles: userRoles);

        Role role = core.Set<Role>().IgnoreQueryFilters().Single(predicate: found => found.Id == seededContext.RoleId);
        await core.DeleteAsync(role: role);

        App app = core.Set<App>().IgnoreQueryFilters().Single(predicate: found => found.Id == seededContext.AppId);
        await core.DeleteAsync(app: app);

    }

    private async Task<int> GetFlowDefinitionCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(expected: HttpStatusCode.OK, because: content);
        return int.Parse(s: content);
    }

    private async Task<IReadOnlyList<FlowDefinition>> GetFlowDefinitionsAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(expected: HttpStatusCode.OK, because: content);
        return JsonSerializer.Deserialize<ODataEnvelope<FlowDefinition>>(json: content, options: JsonOptions)!.Value;
    }

    private async Task<string> GetKnownActivityTypesAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}/KnownActivityTypes()");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(expected: HttpStatusCode.OK, because: content);
        return content;
    }

    private async Task<string> GetKnownSystemTypesAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}/KnownSystemTypes()");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(expected: HttpStatusCode.OK, because: content);
        return content;
    }

    private async Task<int> ExecuteFlowDefinitionAsync(Guid id, string payload)
    {
        using HttpRequestMessage request = new(HttpMethod.Post, $"{BaseUrl}({id})/Execute")
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json"),
        };

        using HttpResponseMessage response = await Client.SendAsync(request: request);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(expected: HttpStatusCode.OK, because: content);
        return (int)response.StatusCode;
    }
    private async Task<int> GetFlowDefinitionStatusCodeAsync(Guid id)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}({id})");
        return (int)response.StatusCode;
    }
}