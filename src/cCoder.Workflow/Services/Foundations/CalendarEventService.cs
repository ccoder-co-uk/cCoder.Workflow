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
        TryCatch(operation: () => { ValidateAllOnGet(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<CalendarEvent> ExecuteGetAll(bool ignoreFilters = false)
    {
        if (ignoreFilters)
        {
            return calendarEventBroker
                .SelectAllCalendarEventsIgnoringQueryFilters();
        }

        return calendarEventBroker.SelectAllCalendarEvents();
    }

    public ValueTask<CalendarEvent> AddCalendarEventAsync(CalendarEvent newCalendarEvent) =>
        TryCatch(operation: async () => { ValidateCalendarEventOnAdd(inputs: [newCalendarEvent]); return await ExecuteAddAsync(calendarEvent: newCalendarEvent); }, isValueTask: true);

    private async ValueTask<CalendarEvent> ExecuteAddAsync(CalendarEvent calendarEvent)
    {
        authorizationBroker.Authorize(
appId: calendarEventBroker.SelectAppId(entity: calendarEvent),
privilege: $"{nameof(CalendarEvent)}_create"
        );

        CalendarEvent newCalendarEvent = CreateStorageCalendarEvent(item: calendarEvent);

        CalendarEvent result = await calendarEventBroker.InsertCalendarEventAsync(newEntity: newCalendarEvent);
        calendarEvent.Id = result.Id;
        calendarEvent.Name = result.Name;
        calendarEvent.Description = result.Description;
        calendarEvent.Start = result.Start;
        calendarEvent.DurationInTicks = result.DurationInTicks;
        calendarEvent.CalendarId = result.CalendarId;
        return calendarEvent;
    }

    public ValueTask<CalendarEvent> UpdateCalendarEventAsync(CalendarEvent updatedCalendarEvent) =>
        TryCatch(operation: async () => { ValidateCalendarEventOnUpdate(inputs: [updatedCalendarEvent]); return await ExecuteUpdateAsync(calendarEvent: updatedCalendarEvent); }, isValueTask: true);

    private async ValueTask<CalendarEvent> ExecuteUpdateAsync(CalendarEvent calendarEvent)
    {
        authorizationBroker.Authorize(
appId: calendarEventBroker.SelectAppId(entity: calendarEvent),
privilege: $"{nameof(CalendarEvent)}_update"
        );

        CalendarEvent updateCalendarEvent = CreateStorageCalendarEvent(item: calendarEvent);

        CalendarEvent result = await calendarEventBroker.UpdateCalendarEventAsync(
updatedEntity: updateCalendarEvent
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
appId: calendarEventBroker.SelectAppId(entity: calendarEvent),
privilege: $"{nameof(CalendarEvent)}_delete"
        );

        _ = await calendarEventBroker.DeleteCalendarEventAsync(
deletedEntity: CreateStorageCalendarEvent(item: calendarEvent)
        );
    }

    public ValueTask DeleteAllForAppCalendarEventAsync(IEnumerable<CalendarEvent> deletedItems) =>
        TryCatch(operation: async () => { ValidateAllForAppCalendarEventOnDelete(inputs: [deletedItems]); await ExecuteDeleteAllForAppAsync(items: deletedItems); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllForAppAsync(IEnumerable<CalendarEvent> items)
    {
        IEnumerable<CalendarEvent> storageCalendarEvents =
            items?.Select(selector: CreateStorageCalendarEvent) ?? [];

        if (!storageCalendarEvents.Any())
        {
            return ValueTask.CompletedTask;
        }

        return calendarEventBroker.DeleteAllCalendarEventsAsync(
            deletedItems: storageCalendarEvents);
    }

    public ValueTask DeleteAllByAppIdAsync(int appId) =>
        TryCatch(operation: async () => { ValidateAllByAppIdOnDelete(inputs: [appId]); await ExecuteDeleteAllByAppIdAsync(appId: appId); }, isValueTask: true);

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