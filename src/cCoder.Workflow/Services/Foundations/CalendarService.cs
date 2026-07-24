// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Brokers.Storage;
using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Services.Foundations;

internal class CalendarService(
    ICalendarBroker calendarBroker,
    IAuthorizationBroker authorizationBroker
) : ICalendarService
{
    public Calendar Get(int id)
    {
        Calendar calendar = GetAll().FirstOrDefault(i => i.Id == id);
        if (calendar is not null)
            return calendar;

        Calendar unrestrictedCalendar = GetAll(true).FirstOrDefault(i => i.Id == id);
        if (unrestrictedCalendar is not null)
            throw new SecurityException("Access Denied!");

        return null;
    }

    public IQueryable<Calendar> GetAll(bool ignoreFilters = false) =>
        calendarBroker.GetAllCalendars(ignoreFilters);

    public async ValueTask<Calendar> AddAsync(Calendar calendar)
    {
        authorizationBroker.Authorize(calendar.AppId, $"{nameof(Calendar)}_create");
        Calendar newCalendar = CreateStorageCalendar(calendar);

        Calendar result = await calendarBroker.AddCalendarAsync(newCalendar);
        calendar.Id = result.Id;
        calendar.AppId = result.AppId;
        calendar.Name = result.Name;
        calendar.Description = result.Description;
        return calendar;
    }

    public async ValueTask<Calendar> UpdateAsync(Calendar calendar)
    {
        authorizationBroker.Authorize(calendar.AppId, $"{nameof(Calendar)}_update");
        Calendar updateCalendar = CreateStorageCalendar(calendar);

        Calendar result = await calendarBroker.UpdateCalendarAsync(updateCalendar);
        calendar.Id = result.Id;
        calendar.AppId = result.AppId;
        calendar.Name = result.Name;
        calendar.Description = result.Description;
        return calendar;
    }

    public async ValueTask DeleteAsync(int id)
    {
        Calendar calendar = GetAll(ignoreFilters: true).FirstOrDefault(item => item.Id == id);

        if (calendar is null)
            return;

        authorizationBroker.Authorize(calendar.AppId, $"{nameof(Calendar)}_delete");
        _ = await calendarBroker.DeleteCalendarAsync(CreateStorageCalendar(calendar));
    }

    public ValueTask DeleteAllForAppAsync(IEnumerable<Calendar> items) =>
        calendarBroker.DeleteAllCalendarsAsync(
            items?.Select(CreateStorageCalendar) ?? []);

    public ValueTask DeleteAllByAppIdAsync(int appId) =>
        calendarBroker.DeleteAllCalendarsByAppIdAsync(appId);

    private static Calendar CreateStorageCalendar(Calendar item) =>
        item == null
            ? null
            : new()
            {
                Id = item.Id,
                AppId = item.AppId,
                Name = item.Name,
                Description = item.Description,
                App = item.App,
                Events = item.Events,
            };
}