// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using cCoder.Data;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using PlanningCalendar = cCoder.Data.Models.Planning.Calendar;


using Web.AcceptanceTests.Infrastructure;
namespace Web.AcceptanceTests.Tests.Workflow.Planning;

public sealed partial class CalendarControllerTests
{
    [Fact]
    public async Task GetCount_ReturnsNonNegativeCount()
    {
        // Given

        // When
        int actualCount = await GetCalendarCountAsync();

        // Then
        actualCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task Get_ReturnsListOfCalendars()
    {
        // Given

        // When
        IReadOnlyList<PlanningCalendar> actualCalendars = await GetCalendarsAsync(1);

        // Then
        actualCalendars.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_ReturnsCalendarById()
    {
        // Given
        SeededCalendarContext seededContext = await SeedDatabase();
        string name = Unique("Calendar");
        PlanningCalendar expectedCalendar = await CreateCalendarAsync(new
        {
            appId = seededContext.AppId,
            name,
            description = "Acceptance calendar",
        });
        PlanningCalendar actualCalendar;

        // When
        actualCalendar = await GetCalendarAsync(expectedCalendar.Id);

        // Then
        actualCalendar.Should().NotBeNull();
        actualCalendar.Id.Should().Be(expectedCalendar.Id);
        actualCalendar.Name.Should().Be(name);

        await DeleteCalendarAsync(expectedCalendar.Id);
        await Teardown(seededContext);
    }

    [Fact]
    public async Task Get_WithoutReadPrivilege_ReturnsNotFound()
    {
        SeededCalendarContext seededContext = await SeedDatabase("calendar_create", "calendar_update", "calendar_delete");

        using IServiceScope scope = fixture.Factory.Services.CreateScope();
        using var core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        PlanningCalendar hiddenCalendar = await core.AddPlanningCalendarAsync(new PlanningCalendar
        {
            AppId = seededContext.AppId,
            Name = Unique("HiddenCalendar"),
            Description = "Hidden calendar",
        });

        int actualStatusCode = await GetCalendarStatusCodeAsync(hiddenCalendar.Id);

        actualStatusCode.Should().Be((int)HttpStatusCode.NotFound);

        await Teardown(seededContext);
    }
}