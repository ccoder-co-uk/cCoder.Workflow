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

    private IQueryable<Calendar> ExecuteGetAll(bool ignoreFilters = false)
    {
        if (ignoreFilters)
        {
            return calendarBroker.SelectAllCalendarsIgnoringQueryFilters();
        }

        return calendarBroker.SelectAllCalendars();
    }

    public ValueTask<Calendar> AddCalendarAsync(Calendar newCalendar) =>
        TryCatch(operation: async () => { ValidateCalendarOnAdd(inputs: [newCalendar]); return await ExecuteAddAsync(calendar: newCalendar); }, isValueTask: true);

    private async ValueTask<Calendar> ExecuteAddAsync(Calendar calendar)
    {
        authorizationBroker.Authorize(appId: calendar.AppId, privilege: $"{nameof(Calendar)}_create");
        Calendar newCalendar = CreateStorageCalendar(item: calendar);

        Calendar result = await calendarBroker.InsertCalendarAsync(newEntity: newCalendar);
        calendar.Id = result.Id;
        calendar.AppId = result.AppId;
        calendar.Name = result.Name;
        calendar.Description = result.Description;
        return calendar;
    }

    public ValueTask<Calendar> UpdateCalendarAsync(Calendar updatedCalendar) =>
        TryCatch(operation: async () => { ValidateCalendarOnUpdate(inputs: [updatedCalendar]); return await ExecuteUpdateAsync(calendar: updatedCalendar); }, isValueTask: true);

    private async ValueTask<Calendar> ExecuteUpdateAsync(Calendar calendar)
    {
        authorizationBroker.Authorize(appId: calendar.AppId, privilege: $"{nameof(Calendar)}_update");
        Calendar updateCalendar = CreateStorageCalendar(item: calendar);

        Calendar result = await calendarBroker.UpdateCalendarAsync(updatedEntity: updateCalendar);
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
        _ = await calendarBroker.DeleteCalendarAsync(deletedEntity: CreateStorageCalendar(item: calendar));
    }

    public ValueTask DeleteAllForAppCalendarAsync(IEnumerable<Calendar> deletedItems) =>
        TryCatch(operation: async () => { ValidateAllForAppCalendarOnDelete(inputs: [deletedItems]); await ExecuteDeleteAllForAppAsync(items: deletedItems); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllForAppAsync(IEnumerable<Calendar> items)
    {
        IEnumerable<Calendar> storageCalendars =
            items?.Select(selector: CreateStorageCalendar) ?? [];

        if (!storageCalendars.Any())
        {
            return ValueTask.CompletedTask;
        }

        return calendarBroker.DeleteAllCalendarsAsync(
            deletedItems: storageCalendars);
    }

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