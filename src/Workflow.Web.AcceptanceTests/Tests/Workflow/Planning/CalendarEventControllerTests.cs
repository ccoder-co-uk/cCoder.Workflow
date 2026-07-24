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
public sealed partial class CalendarEventControllerTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
    private string BaseUrl { get; } = "/Api/Workflow/CalendarEvent";
    private static JsonSerializerOptions JsonOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";

    private sealed record SeededCalendarEventContext(int AppId, Guid RoleId, int CalendarId);
    private sealed record ODataEnvelope<T>(List<T> Value);

    private async Task<SeededCalendarEventContext> SeedDatabase()
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        App app = await core.AddAppAsync(app: new App
        {
            Name = Unique(prefix: "AcceptanceApp"),
            Domain = $"{Unique(prefix: "calendarevent")}.local",
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
            Privs = "app_admin,calendar_create,calendar_update,calendar_delete,calendar_read,calendarevent_create,calendarevent_update,calendarevent_delete,calendarevent_read",
        });

        await core.AddUserRoleAsync(userRole: new UserRole { RoleId = role.Id, UserId = "Guest" });

        Calendar calendar = await core.AddCalendarAsync(calendar: new Calendar
        {
            AppId = app.Id,
            Name = Unique(prefix: "Calendar"),
            Description = "Acceptance calendar",
        });

        return new SeededCalendarEventContext(app.Id, role.Id, calendar.Id);
    }

    private async Task<CalendarEvent> CreateCalendarEventAsync(object payload)
    {
        using HttpResponseMessage response = await Client.PostAsJsonAsync(requestUri: BaseUrl, value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<CalendarEvent>(json: content, options: JsonOptions)
            ?? throw new InvalidOperationException("Expected calendar event payload.");
    }

    private async Task<int> UpdateCalendarEventAsync(int calendarEventId, object payload)
    {
        using HttpResponseMessage response = await Client.PutAsJsonAsync(requestUri: $"{BaseUrl}({calendarEventId})", value: payload);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<int> PatchCalendarEventAsync(int calendarEventId, object payload)
    {
        using HttpRequestMessage request = new(HttpMethod.Patch, $"{BaseUrl}({calendarEventId})")
        {
            Content = JsonContent.Create(inputValue: payload),
        };

        using HttpResponseMessage response = await Client.SendAsync(request: request);
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<int> DeleteCalendarEventAsync(int calendarEventId)
    {
        using HttpResponseMessage response = await Client.DeleteAsync(requestUri: $"{BaseUrl}({calendarEventId})");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return (int)response.StatusCode;
    }

    private async Task<CalendarEvent> GetCalendarEventAsync(int calendarEventId)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}({calendarEventId})");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<CalendarEvent>(json: content, options: JsonOptions)
            ?? throw new InvalidOperationException("Expected calendar event payload.");
    }

    private async Task Teardown(SeededCalendarEventContext seededContext)
    {
        using IServiceScope scope = fixture.Factory.Services.CreateScope();

        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        CalendarEvent[] calendarEvents = core.Set<CalendarEvent>()
            .IgnoreQueryFilters()
            .Where(predicate: calendarEvent => calendarEvent.CalendarId == seededContext.CalendarId)
            .ToArray();

        await core.DeleteAllAsync(calendarEvents: calendarEvents);

        Calendar calendar = core.Set<Calendar>()
            .IgnoreQueryFilters()
            .Single(predicate: found => found.Id == seededContext.CalendarId);

        await core.DeleteAsync(calendar: calendar);

        UserRole[] userRoles = core.Set<UserRole>()
            .IgnoreQueryFilters()
            .Where(predicate: userRole => userRole.RoleId == seededContext.RoleId)
            .ToArray();

        await core.DeleteAllAsync(userRoles: userRoles);

        Role role = core.Set<Role>()
            .IgnoreQueryFilters()
            .Single(predicate: found => found.Id == seededContext.RoleId);

        await core.DeleteAsync(role: role);

        App app = core.Set<App>()
            .IgnoreQueryFilters()
            .Single(predicate: found => found.Id == seededContext.AppId);

        await core.DeleteAsync(app: app);
    }

    private async Task<int> GetCalendarEventCountAsync()
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}/$count");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return int.Parse(s: content);
    }

    private async Task<IReadOnlyList<CalendarEvent>> GetCalendarEventsAsync(int top)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}?$top={top}");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK, because: content);

        return JsonSerializer.Deserialize<ODataEnvelope<CalendarEvent>>(json: content, options: JsonOptions)?.Value
            ?? throw new InvalidOperationException("Expected calendar event OData payload.");
    }
    private async Task<int> GetCalendarEventStatusCodeAsync(int calendarEventId)
    {
        using HttpResponseMessage response = await Client.GetAsync(requestUri: $"{BaseUrl}({calendarEventId})");
        return (int)response.StatusCode;
    }
}