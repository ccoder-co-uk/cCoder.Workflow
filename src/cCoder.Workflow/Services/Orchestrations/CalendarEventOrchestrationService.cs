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

internal sealed partial class CalendarEventOrchestrationService(ICalendarEventProcessingService processingService, ICalendarEventEventProcessingService eventService) : ICalendarEventOrchestrationService
{
    public CalendarEvent Get(int calendarEventId) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [calendarEventId]); return ExecuteGet(calendarEventId: calendarEventId); });

    private CalendarEvent ExecuteGet(int calendarEventId)
    {
        return processingService.Get(calendarEventId: calendarEventId);
    }

    public IQueryable<CalendarEvent> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<CalendarEvent> ExecuteGetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask<CalendarEvent> AddCalendarEventAsync(CalendarEvent newCalendarEvent) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [newCalendarEvent]); return await ExecuteAddAsync(entity: newCalendarEvent); }, isValueTask: true);

    private async ValueTask<CalendarEvent> ExecuteAddAsync(CalendarEvent entity)
    {
        CalendarEvent result = await processingService.AddCalendarEventAsync(newCalendarEvent: entity);
        await eventService.RaiseCalendarEventAddEventAsync(calendarEvent: result);
        return result;
    }

    public ValueTask<CalendarEvent> UpdateCalendarEventAsync(CalendarEvent updatedCalendarEvent) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [updatedCalendarEvent]); return await ExecuteUpdateAsync(entity: updatedCalendarEvent); }, isValueTask: true);

    private async ValueTask<CalendarEvent> ExecuteUpdateAsync(CalendarEvent entity)
    {
        CalendarEvent result = await processingService.UpdateCalendarEventAsync(updatedCalendarEvent: entity);
        await eventService.RaiseCalendarEventUpdateEventAsync(calendarEvent: result);
        return result;
    }

    public ValueTask DeleteAsync(int calendarEventId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [calendarEventId]); await ExecuteDeleteAsync(calendarEventId: calendarEventId); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAsync(int calendarEventId)
    {
        CalendarEvent entity = processingService.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: item => item.Id == calendarEventId);

        if (entity is null)
        {
            return;
        }

        await eventService.RaiseCalendarEventDeleteEventAsync(calendarEvent: entity);
        await processingService.DeleteAsync(calendarEventId: calendarEventId);
    }

    public ValueTask<IEnumerable<Result<CalendarEvent>>> AddOrUpdateCalendarEvent(IEnumerable<CalendarEvent> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); return await ExecuteAddOrUpdate(items: items); }, isValueTask: true);

    private ValueTask<IEnumerable<Result<CalendarEvent>>> ExecuteAddOrUpdate(IEnumerable<CalendarEvent> items)
    {
        return processingService.AddOrUpdateCalendarEvent(items: items);
    }

    public ValueTask DeleteAllCalendarEventAsync(IEnumerable<CalendarEvent> deletedItems) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [deletedItems]); await ExecuteDeleteAllAsync(items: deletedItems); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllAsync(IEnumerable<CalendarEvent> items)
    {
        return processingService.DeleteAllCalendarEventAsync(deletedItems: items);
    }
}