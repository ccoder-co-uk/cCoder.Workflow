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

internal class CalendarEventOrchestrationService(ICalendarEventProcessingService processingService, ICalendarEventEventProcessingService eventService) : ICalendarEventOrchestrationService
{
    public CalendarEvent Get(int calendarEventId)
    {
        return processingService.Get(calendarEventId: calendarEventId);
    }

    public IQueryable<CalendarEvent> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters: ignoreFilters);
    }

    public async ValueTask<CalendarEvent> AddAsync(CalendarEvent entity)
    {
        CalendarEvent result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseCalendarEventAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<CalendarEvent> UpdateAsync(CalendarEvent entity)
    {
        CalendarEvent result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaiseCalendarEventUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(int calendarEventId)
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

    public ValueTask<IEnumerable<Result<CalendarEvent>>> AddOrUpdate(IEnumerable<CalendarEvent> items)
    {
        return processingService.AddOrUpdate(items: items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<CalendarEvent> items)
    {
        return processingService.DeleteAllAsync(items: items);
    }
}