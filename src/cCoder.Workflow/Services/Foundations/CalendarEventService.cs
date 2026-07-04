using System.Security;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Brokers.Storage;
using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Services.Foundations;

internal class CalendarEventService(
    ICalendarEventBroker calendarEventBroker,
    IAuthorizationBroker authorizationBroker
) : ICalendarEventService
{
    public CalendarEvent Get(int id)
    {
        CalendarEvent calendarEvent = GetAll().FirstOrDefault(i => i.Id == id);
        if (calendarEvent is not null)
            return calendarEvent;

        CalendarEvent unrestrictedCalendarEvent = GetAll(true).FirstOrDefault(i => i.Id == id);
        if (unrestrictedCalendarEvent is not null)
            throw new SecurityException("Access Denied!");

        return null;
    }

    public IQueryable<CalendarEvent> GetAll(bool ignoreFilters = false) =>
        calendarEventBroker.GetAllCalendarEvents(ignoreFilters);

    public async ValueTask<CalendarEvent> AddAsync(CalendarEvent calendarEvent)
    {
        authorizationBroker.Authorize(
            calendarEventBroker.GetAppId(calendarEvent),
            $"{nameof(CalendarEvent)}_create"
        );
        CalendarEvent newCalendarEvent = CreateStorageCalendarEvent(calendarEvent);

        CalendarEvent result = await calendarEventBroker.AddCalendarEventAsync(newCalendarEvent);
        calendarEvent.Id = result.Id;
        calendarEvent.Name = result.Name;
        calendarEvent.Description = result.Description;
        calendarEvent.Start = result.Start;
        calendarEvent.DurationInTicks = result.DurationInTicks;
        calendarEvent.CalendarId = result.CalendarId;
        return calendarEvent;
    }

    public async ValueTask<CalendarEvent> UpdateAsync(CalendarEvent calendarEvent)
    {
        authorizationBroker.Authorize(
            calendarEventBroker.GetAppId(calendarEvent),
            $"{nameof(CalendarEvent)}_update"
        );
        CalendarEvent updateCalendarEvent = CreateStorageCalendarEvent(calendarEvent);

        CalendarEvent result = await calendarEventBroker.UpdateCalendarEventAsync(
            updateCalendarEvent
        );
        calendarEvent.Id = result.Id;
        calendarEvent.Name = result.Name;
        calendarEvent.Description = result.Description;
        calendarEvent.Start = result.Start;
        calendarEvent.DurationInTicks = result.DurationInTicks;
        calendarEvent.CalendarId = result.CalendarId;
        return calendarEvent;
    }

    public async ValueTask DeleteAsync(int id)
    {
        CalendarEvent calendarEvent = GetAll(ignoreFilters: true).FirstOrDefault(item => item.Id == id);

        if (calendarEvent is null)
            return;

        authorizationBroker.Authorize(
            calendarEventBroker.GetAppId(calendarEvent),
            $"{nameof(CalendarEvent)}_delete"
        );
        _ = await calendarEventBroker.DeleteCalendarEventAsync(
            CreateStorageCalendarEvent(calendarEvent)
        );
    }

    public ValueTask DeleteAllForAppAsync(IEnumerable<CalendarEvent> items) =>
        calendarEventBroker.DeleteAllCalendarEventsAsync(
            items?.Select(CreateStorageCalendarEvent) ?? []);

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
