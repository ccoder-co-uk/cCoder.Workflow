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

    public ValueTask<CalendarEvent> AddAsync(CalendarEvent entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); return await ExecuteAddAsync(entity: entity); }, isValueTask: true);

    private async ValueTask<CalendarEvent> ExecuteAddAsync(CalendarEvent entity)
    {
        CalendarEvent result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseCalendarEventAddEventAsync(entity: result);
        return result;
    }

    public ValueTask<CalendarEvent> UpdateAsync(CalendarEvent entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); return await ExecuteUpdateAsync(entity: entity); }, isValueTask: true);

    private async ValueTask<CalendarEvent> ExecuteUpdateAsync(CalendarEvent entity)
    {
        CalendarEvent result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseCalendarEventUpdateEventAsync(entity: result);
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

        await eventService.RaiseCalendarEventDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(calendarEventId: calendarEventId);
    }

    public ValueTask<IEnumerable<Result<CalendarEvent>>> AddOrUpdate(IEnumerable<CalendarEvent> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); return await ExecuteAddOrUpdate(items: items); }, isValueTask: true);

    private ValueTask<IEnumerable<Result<CalendarEvent>>> ExecuteAddOrUpdate(IEnumerable<CalendarEvent> items)
    {
        return processingService.AddOrUpdate(items: items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<CalendarEvent> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); await ExecuteDeleteAllAsync(items: items); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllAsync(IEnumerable<CalendarEvent> items)
    {
        return processingService.DeleteAllAsync(items: items);
    }
}