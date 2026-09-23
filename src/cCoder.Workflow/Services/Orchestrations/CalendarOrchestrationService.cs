// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Brokers.Loggings;
using cCoder.Workflow.Services.Processings;

namespace cCoder.Workflow.Services.Orchestrations;

internal sealed partial class CalendarOrchestrationService(
    ICalendarProcessingService processingService,
    ICalendarEventProcessingService calendarEventProcessingService,
    ICalendarEntityEventProcessingService eventService,
    ILoggingBroker loggingBroker)
    : ICalendarOrchestrationService
{
    public object CreateSingleResult<T>(IQueryable<T> queryable) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [queryable]);

            return processingService.CreateSingleResult(queryable: queryable);
        });

    public void LogError(Exception exception, string message) =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: [exception, message]);

            loggingBroker.LogError(
                exception: exception,
                message: message);

            return true;
        });

    public Calendar Get(int calendarId) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [calendarId]); return ExecuteGet(calendarId: calendarId); });

    private Calendar ExecuteGet(int calendarId)
    {
        return processingService.Get(calendarId: calendarId);
    }

    public IQueryable<Calendar> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () => { ValidateAllOnGet(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<Calendar> ExecuteGetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask<Calendar> AddCalendarAsync(Calendar newCalendar) =>
        TryCatch(operation: async () => { ValidateCalendarOnAdd(inputs: [newCalendar]); return await ExecuteAddAsync(entity: newCalendar); }, isValueTask: true);

    private async ValueTask<Calendar> ExecuteAddAsync(Calendar entity)
    {
        Calendar result = await processingService.AddCalendarAsync(newCalendar: entity);
        await eventService.RaiseCalendarAddEventAsync(calendar: result);
        return result;
    }

    public ValueTask<Calendar> UpdateCalendarAsync(Calendar updatedCalendar) =>
        TryCatch(operation: async () => { ValidateCalendarOnUpdate(inputs: [updatedCalendar]); return await ExecuteUpdateAsync(entity: updatedCalendar); }, isValueTask: true);

    private async ValueTask<Calendar> ExecuteUpdateAsync(Calendar entity)
    {
        Calendar result = await processingService.UpdateCalendarAsync(updatedCalendar: entity);
        await eventService.RaiseCalendarUpdateEventAsync(calendar: result);
        return result;
    }

    public ValueTask DeleteAsync(int calendarId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [calendarId]); await ExecuteDeleteAsync(calendarId: calendarId); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAsync(int calendarId)
    {
        Calendar entity = processingService.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == calendarId);

        if (entity is null)
        {
            return;
        }

        CalendarEvent[] calendarEvents =
            calendarEventProcessingService.GetAll()
                .Where(predicate: calendarEvent =>
                    calendarEvent.CalendarId == entity.Id)
                .ToArray();

        await calendarEventProcessingService.DeleteAllCalendarEventAsync(
            deletedItems: calendarEvents);

        await eventService.RaiseCalendarDeleteEventAsync(calendar: entity);
        await processingService.DeleteAsync(calendarId: calendarId);
    }

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: async () => { ValidateByAppIdOnDelete(inputs: [appId]); await ExecuteDeleteByAppIdAsync(appId: appId); }, isValueTask: true);

    private async ValueTask ExecuteDeleteByAppIdAsync(int appId)
    {
        await calendarEventProcessingService.DeleteAllByAppIdAsync(appId: appId);
        await processingService.DeleteByAppIdAsync(appId: appId);
    }

    public ValueTask<IEnumerable<Result<Calendar>>> AddOrUpdateCalendar(IEnumerable<Calendar> items) =>
        TryCatch(operation: async () => { ValidateOrUpdateCalendarOnAdd(inputs: [items]); return await ExecuteAddOrUpdate(items: items); }, isValueTask: true);

    private ValueTask<IEnumerable<Result<Calendar>>> ExecuteAddOrUpdate(IEnumerable<Calendar> items)
    {
        return processingService.AddOrUpdateCalendar(items: items);
    }

    public ValueTask DeleteAllCalendarAsync(IEnumerable<Calendar> deletedItems) =>
        TryCatch(operation: async () => { ValidateAllCalendarOnDelete(inputs: [deletedItems]); await ExecuteDeleteAllAsync(items: deletedItems); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllAsync(IEnumerable<Calendar> items)
    {
        return processingService.DeleteAllCalendarAsync(deletedItems: items);
    }
}