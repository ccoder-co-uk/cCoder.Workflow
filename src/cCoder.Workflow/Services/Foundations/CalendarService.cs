// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Brokers.Storage;
using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class CalendarService(
    ICalendarBroker calendarBroker,
    IAuthorizationBroker authorizationBroker
) : ICalendarService
{
    public Calendar Get(int calendarId) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [calendarId]); return ExecuteGet(calendarId: calendarId); });

    private Calendar ExecuteGet(int calendarId)
    {
        Calendar calendar = GetAll()
            .FirstOrDefault(predicate: i => i.Id == calendarId);

        if (calendar is not null)
        {
            return calendar;
        }

        Calendar unrestrictedCalendar = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: i => i.Id == calendarId);

        if (unrestrictedCalendar is not null)
        {
            throw new SecurityException("Access Denied!");
        }

        return null;
    }

    public IQueryable<Calendar> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () => { ValidateAllOnGet(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<Calendar> ExecuteGetAll(bool ignoreFilters = false) =>
        calendarBroker.GetAllCalendars(ignoreFilters: ignoreFilters);

    public ValueTask<Calendar> AddAsync(Calendar calendar) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [calendar]); return await ExecuteAddAsync(calendar: calendar); }, isValueTask: true);

    private async ValueTask<Calendar> ExecuteAddAsync(Calendar calendar)
    {
        authorizationBroker.Authorize(appId: calendar.AppId, privilege: $"{nameof(Calendar)}_create");
        Calendar newCalendar = CreateStorageCalendar(item: calendar);

        Calendar result = await calendarBroker.AddCalendarAsync(entity: newCalendar);
        calendar.Id = result.Id;
        calendar.AppId = result.AppId;
        calendar.Name = result.Name;
        calendar.Description = result.Description;
        return calendar;
    }

    public ValueTask<Calendar> UpdateAsync(Calendar calendar) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [calendar]); return await ExecuteUpdateAsync(calendar: calendar); }, isValueTask: true);

    private async ValueTask<Calendar> ExecuteUpdateAsync(Calendar calendar)
    {
        authorizationBroker.Authorize(appId: calendar.AppId, privilege: $"{nameof(Calendar)}_update");
        Calendar updateCalendar = CreateStorageCalendar(item: calendar);

        Calendar result = await calendarBroker.UpdateCalendarAsync(entity: updateCalendar);
        calendar.Id = result.Id;
        calendar.AppId = result.AppId;
        calendar.Name = result.Name;
        calendar.Description = result.Description;
        return calendar;
    }

    public ValueTask DeleteAsync(int calendarId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [calendarId]); await ExecuteDeleteAsync(calendarId: calendarId); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAsync(int calendarId)
    {
        Calendar calendar = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == calendarId);

        if (calendar is null)
        {
            return;
        }

        authorizationBroker.Authorize(appId: calendar.AppId, privilege: $"{nameof(Calendar)}_delete");
        _ = await calendarBroker.DeleteCalendarAsync(entity: CreateStorageCalendar(item: calendar));
    }

    public ValueTask DeleteAllForAppAsync(IEnumerable<Calendar> items) =>
        TryCatch(operation: async () => { ValidateAllForAppOnDelete(inputs: [items]); await ExecuteDeleteAllForAppAsync(items: items); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllForAppAsync(IEnumerable<Calendar> items) =>
        calendarBroker.DeleteAllCalendarsAsync(
    items: items?.Select(selector: CreateStorageCalendar) ?? []);

    public ValueTask DeleteAllByAppIdAsync(int appId) =>
        TryCatch(operation: async () => { ValidateAllByAppIdOnDelete(inputs: [appId]); await ExecuteDeleteAllByAppIdAsync(appId: appId); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllByAppIdAsync(int appId) =>
        calendarBroker.DeleteAllCalendarsByAppIdAsync(appId: appId);

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