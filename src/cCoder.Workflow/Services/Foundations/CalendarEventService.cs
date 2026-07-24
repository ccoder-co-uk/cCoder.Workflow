// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Brokers.Storage;
using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class CalendarEventService(
    ICalendarEventBroker calendarEventBroker,
    IAuthorizationBroker authorizationBroker
) : ICalendarEventService
{
    public CalendarEvent Get(int calendarEventId) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [calendarEventId]); return ExecuteGet(calendarEventId: calendarEventId); });

    private CalendarEvent ExecuteGet(int calendarEventId)
    {
        CalendarEvent calendarEvent = GetAll()
            .FirstOrDefault(predicate: i => i.Id == calendarEventId);

        if (calendarEvent is not null)
        {
            return calendarEvent;
        }

        CalendarEvent unrestrictedCalendarEvent = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: i => i.Id == calendarEventId);

        if (unrestrictedCalendarEvent is not null)
        {
            throw new SecurityException("Access Denied!");
        }

        return null;
    }

    public IQueryable<CalendarEvent> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<CalendarEvent> ExecuteGetAll(bool ignoreFilters = false) =>
        calendarEventBroker.GetAllCalendarEvents(ignoreFilters: ignoreFilters);

    public ValueTask<CalendarEvent> AddAsync(CalendarEvent calendarEvent) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [calendarEvent]); return await ExecuteAddAsync(calendarEvent: calendarEvent); }, isValueTask: true);

    private async ValueTask<CalendarEvent> ExecuteAddAsync(CalendarEvent calendarEvent)
    {
        authorizationBroker.Authorize(
appId: calendarEventBroker.GetAppId(entity: calendarEvent),
privilege: $"{nameof(CalendarEvent)}_create"
        );

        CalendarEvent newCalendarEvent = CreateStorageCalendarEvent(item: calendarEvent);

        CalendarEvent result = await calendarEventBroker.AddCalendarEventAsync(entity: newCalendarEvent);
        calendarEvent.Id = result.Id;
        calendarEvent.Name = result.Name;
        calendarEvent.Description = result.Description;
        calendarEvent.Start = result.Start;
        calendarEvent.DurationInTicks = result.DurationInTicks;
        calendarEvent.CalendarId = result.CalendarId;
        return calendarEvent;
    }

    public ValueTask<CalendarEvent> UpdateAsync(CalendarEvent calendarEvent) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [calendarEvent]); return await ExecuteUpdateAsync(calendarEvent: calendarEvent); }, isValueTask: true);

    private async ValueTask<CalendarEvent> ExecuteUpdateAsync(CalendarEvent calendarEvent)
    {
        authorizationBroker.Authorize(
appId: calendarEventBroker.GetAppId(entity: calendarEvent),
privilege: $"{nameof(CalendarEvent)}_update"
        );

        CalendarEvent updateCalendarEvent = CreateStorageCalendarEvent(item: calendarEvent);

        CalendarEvent result = await calendarEventBroker.UpdateCalendarEventAsync(
entity: updateCalendarEvent
        );

        calendarEvent.Id = result.Id;
        calendarEvent.Name = result.Name;
        calendarEvent.Description = result.Description;
        calendarEvent.Start = result.Start;
        calendarEvent.DurationInTicks = result.DurationInTicks;
        calendarEvent.CalendarId = result.CalendarId;
        return calendarEvent;
    }

    public ValueTask DeleteAsync(int calendarEventId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [calendarEventId]); await ExecuteDeleteAsync(calendarEventId: calendarEventId); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAsync(int calendarEventId)
    {
        CalendarEvent calendarEvent = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == calendarEventId);

        if (calendarEvent is null)
        {
            return;
        }

        authorizationBroker.Authorize(
appId: calendarEventBroker.GetAppId(entity: calendarEvent),
privilege: $"{nameof(CalendarEvent)}_delete"
        );

        _ = await calendarEventBroker.DeleteCalendarEventAsync(
entity: CreateStorageCalendarEvent(item: calendarEvent)
        );
    }

    public ValueTask DeleteAllForAppAsync(IEnumerable<CalendarEvent> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); await ExecuteDeleteAllForAppAsync(items: items); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllForAppAsync(IEnumerable<CalendarEvent> items) =>
        calendarEventBroker.DeleteAllCalendarEventsAsync(
    items: items?.Select(selector: CreateStorageCalendarEvent) ?? []);

    public ValueTask DeleteAllByAppIdAsync(int appId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [appId]); await ExecuteDeleteAllByAppIdAsync(appId: appId); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllByAppIdAsync(int appId) =>
        calendarEventBroker.DeleteAllCalendarEventsByAppIdAsync(appId: appId);

    private static CalendarEvent CreateStorageCalendarEvent(CalendarEvent item) =>
        item == null
            ? null
            : new()
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Start = item.Start,
                DurationInTicks = item.DurationInTicks,
                CalendarId = item.CalendarId,
                Calendar = item.Calendar,
            };
}