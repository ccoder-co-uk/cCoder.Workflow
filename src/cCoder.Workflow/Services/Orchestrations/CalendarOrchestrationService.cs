// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Processings;

namespace cCoder.Workflow.Services.Orchestrations;

internal sealed partial class CalendarOrchestrationService(
    ICalendarProcessingService processingService,
    ICalendarEventProcessingService calendarEventProcessingService,
    ICalendarEntityEventProcessingService eventService)
    : ICalendarOrchestrationService
{
    public Calendar Get(int calendarId) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [calendarId]); return ExecuteGet(calendarId: calendarId); });

    private Calendar ExecuteGet(int calendarId)
    {
        return processingService.Get(calendarId: calendarId);
    }

    public IQueryable<Calendar> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<Calendar> ExecuteGetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask<Calendar> AddCalendarAsync(Calendar newEntity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [newEntity]); return await ExecuteAddAsync(entity: newEntity); }, isValueTask: true);

    private async ValueTask<Calendar> ExecuteAddAsync(Calendar entity)
    {
        Calendar result = await processingService.AddCalendarAsync(newEntity: entity);
        await eventService.RaiseCalendarAddEventAsync(entity: result);
        return result;
    }

    public ValueTask<Calendar> UpdateCalendarAsync(Calendar updatedEntity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [updatedEntity]); return await ExecuteUpdateAsync(entity: updatedEntity); }, isValueTask: true);

    private async ValueTask<Calendar> ExecuteUpdateAsync(Calendar entity)
    {
        Calendar result = await processingService.UpdateCalendarAsync(updatedEntity: entity);
        await eventService.RaiseCalendarUpdateEventAsync(entity: result);
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
        await eventService.RaiseCalendarDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(calendarId: calendarId);
    }

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [appId]); await ExecuteDeleteByAppIdAsync(appId: appId); }, isValueTask: true);

    private async ValueTask ExecuteDeleteByAppIdAsync(int appId)
    {
        await calendarEventProcessingService.DeleteAllByAppIdAsync(appId: appId);
        await processingService.DeleteByAppIdAsync(appId: appId);
    }

    public ValueTask<IEnumerable<Result<Calendar>>> AddOrUpdateCalendar(IEnumerable<Calendar> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); return await ExecuteAddOrUpdate(items: items); }, isValueTask: true);

    private ValueTask<IEnumerable<Result<Calendar>>> ExecuteAddOrUpdate(IEnumerable<Calendar> items)
    {
        return processingService.AddOrUpdateCalendar(items: items);
    }

    public ValueTask DeleteAllCalendarAsync(IEnumerable<Calendar> deletedItems) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [deletedItems]); await ExecuteDeleteAllAsync(items: deletedItems); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllAsync(IEnumerable<Calendar> items)
    {
        return processingService.DeleteAllCalendarAsync(deletedItems: items);
    }
}