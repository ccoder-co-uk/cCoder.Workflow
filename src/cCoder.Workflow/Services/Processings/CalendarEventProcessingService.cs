// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations;

namespace cCoder.Workflow.Services.Processings;

internal sealed partial class CalendarEventProcessingService(ICalendarEventService service) : ICalendarEventProcessingService
{
    public CalendarEvent Get(int calendarEventId) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [calendarEventId]); return ExecuteGet(calendarEventId: calendarEventId); });

    private CalendarEvent ExecuteGet(int calendarEventId)
    {
        return service.Get(calendarEventId: calendarEventId);
    }

    public IQueryable<CalendarEvent> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<CalendarEvent> ExecuteGetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask<CalendarEvent> AddCalendarEventAsync(CalendarEvent newEntity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [newEntity]); return await ExecuteAddAsync(entity: newEntity); }, isValueTask: true);

    private ValueTask<CalendarEvent> ExecuteAddAsync(CalendarEvent entity)
    {
        return service.AddCalendarEventAsync(newCalendarEvent: entity);
    }

    public ValueTask<CalendarEvent> UpdateCalendarEventAsync(CalendarEvent updatedEntity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [updatedEntity]); return await ExecuteUpdateAsync(entity: updatedEntity); }, isValueTask: true);

    private ValueTask<CalendarEvent> ExecuteUpdateAsync(CalendarEvent entity)
    {
        return service.UpdateCalendarEventAsync(updatedCalendarEvent: entity);
    }

    public ValueTask DeleteAsync(int calendarEventId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [calendarEventId]); await ExecuteDeleteAsync(calendarEventId: calendarEventId); }, isValueTask: true);

    private ValueTask ExecuteDeleteAsync(int calendarEventId)
    {
        return service.DeleteAsync(calendarEventId: calendarEventId);
    }

    public ValueTask DeleteAllForAppCalendarEventAsync(IEnumerable<CalendarEvent> deletedItems) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [deletedItems]); await ExecuteDeleteAllForAppAsync(items: deletedItems); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllForAppAsync(IEnumerable<CalendarEvent> items) =>
        service.DeleteAllForAppCalendarEventAsync(deletedItems: items);

    public ValueTask DeleteAllByAppIdAsync(int appId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [appId]); await ExecuteDeleteAllByAppIdAsync(appId: appId); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllByAppIdAsync(int appId) =>
        service.DeleteAllByAppIdAsync(appId: appId);

    public ValueTask<IEnumerable<Result<CalendarEvent>>> AddOrUpdateCalendarEvent(IEnumerable<CalendarEvent> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); return await ExecuteAddOrUpdate(items: items); }, isValueTask: true);

    private async ValueTask<IEnumerable<Result<CalendarEvent>>> ExecuteAddOrUpdate(IEnumerable<CalendarEvent> items)
    {
        List<Result<CalendarEvent>> results = new List<Result<CalendarEvent>>();

        foreach (CalendarEvent item in items)
        {
            try
            {
                CalendarEvent savedItem =
                    item.Id == 0
                        ? await AddCalendarEventAsync(newEntity: item)
                        : await UpdateCalendarEventAsync(updatedEntity: item);

                results.Add(item: new Result<CalendarEvent>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<CalendarEvent>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public ValueTask DeleteAllCalendarEventAsync(IEnumerable<CalendarEvent> deletedItems) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [deletedItems]); await ExecuteDeleteAllAsync(items: deletedItems); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAllAsync(IEnumerable<CalendarEvent> items)
    {
        foreach (CalendarEvent item in items)
        {
            await DeleteAsync(calendarEventId: item.Id);
        }
    }
}