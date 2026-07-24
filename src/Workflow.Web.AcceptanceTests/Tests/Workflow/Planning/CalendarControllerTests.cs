// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Web.AcceptanceTests.Infrastructure;
using Xunit;


using Microsoft.EntityFrameworkCore;
namespace Web.AcceptanceTests.Tests.Workflow.Planning;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class CalendarControllerTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Workflow/Calendar";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededCalendarContext(int AppId, Guid RoleId);
    private sealed record ODataEnvelope<T>(List<T> Value);

    private async Task<SeededCalendarContext> SeedDatabase(params string[] privileges)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        App app = await core.AddAppAsync(new App
        {
            Name = Unique("AcceptanceApp"),
            Domain = $"{Unique("calendar")}.local",
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
                ? "app_admin,calendar_create,calendar_update,calendar_delete,calendar_read"
                : string.Join(',', privileges),
        });

        await core.AddUserRoleAsync(new UserRole { RoleId = role.Id, UserId = "Guest" });

        return new SeededCalendarContext(app.Id, role.Id);
    }

    private async Task<Calendar> CreateCalendarAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(BaseUrl, payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<Calendar>(content, JsonOptions)!;
    }

    private async Task<int> UpdateCalendarAsync(int id, object payload)
    {
        using HttpResponseMessage response = await Client.PutAsJsonAsync($"{BaseUrl}({id})", payload);
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<int> PatchCalendarAsync(int id, object payload)
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

    private async Task<int> DeleteCalendarAsync(int id)
    {
        using HttpResponseMessage response = await Client.DeleteAsync($"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return (int)response.StatusCode;
    }

    private async Task<Calendar> GetCalendarAsync(int id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}({id})");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);

        return JsonSerializer.Deserialize<Calendar>(content, JsonOptions)
            ?? throw new InvalidOperationException("Expected calendar payload.");
    }

    private async Task Teardown(SeededCalendarContext seededContext)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        CalendarEvent[] calendarEvents = core.Set<CalendarEvent>().IgnoreQueryFilters().Where(calendarEvent => calendarEvent.Calendar.AppId == seededContext.AppId).ToArray();
        await core.DeleteAllAsync(calendarEvents);

        Calendar[] calendars = core.Set<Calendar>().IgnoreQueryFilters().Where(calendar => calendar.AppId == seededContext.AppId).ToArray();
        await core.DeleteAllAsync(calendars);

        UserRole[] userRoles = core.Set<UserRole>().IgnoreQueryFilters().Where(userRole => userRole.RoleId == seededContext.RoleId).ToArray();
        await core.DeleteAllAsync(userRoles);

        Role role = core.Set<Role>().IgnoreQueryFilters().Single(found => found.Id == seededContext.RoleId);
        await core.DeleteAsync(role);

        App app = core.Set<App>().IgnoreQueryFilters().Single(found => found.Id == seededContext.AppId);
        await core.DeleteAsync(app);

    }

    private async Task<int> GetCalendarCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return int.Parse(content);
    }

    private async Task<IReadOnlyList<Calendar>> GetCalendarsAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonSerializer.Deserialize<ODataEnvelope<Calendar>>(content, JsonOptions)!.Value;
    }
    private async Task<int> GetCalendarStatusCodeAsync(int id)
    {
        using HttpResponseMessage response = await Client.GetAsync($"{BaseUrl}({id})");
        return (int)response.StatusCode;
    }
}