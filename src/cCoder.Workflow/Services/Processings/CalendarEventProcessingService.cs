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

internal class CalendarEventProcessingService(ICalendarEventService service) : ICalendarEventProcessingService
{
    public CalendarEvent Get(int id)
    {
        return service.Get(id:id);
    }

    public IQueryable<CalendarEvent> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters:ignoreFilters);
    }

    public ValueTask<CalendarEvent> AddAsync(CalendarEvent entity)
    {
        return service.AddAsync(calendarEvent:entity);
    }

    public ValueTask<CalendarEvent> UpdateAsync(CalendarEvent entity)
    {
        return service.UpdateAsync(calendarEvent:entity);
    }

    public ValueTask DeleteAsync(int id)
    {
        return service.DeleteAsync(id:id);
    }

    public ValueTask DeleteAllForAppAsync(IEnumerable<CalendarEvent> items) =>
        service.DeleteAllForAppAsync(items:items);

    public ValueTask DeleteAllByAppIdAsync(int appId) =>
        service.DeleteAllByAppIdAsync(appId:appId);

    public async ValueTask<IEnumerable<Result<CalendarEvent>>> AddOrUpdate(IEnumerable<CalendarEvent> items)
    {
        List<Result<CalendarEvent>> results = new List<Result<CalendarEvent>>();

        foreach (CalendarEvent item in items)
        {
            try
            {
                CalendarEvent savedItem =
                    item.Id == 0
                        ? await AddAsync(entity:item)
                        : await UpdateAsync(entity:item);

                results.Add(item:new Result<CalendarEvent>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item:new Result<CalendarEvent>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<CalendarEvent> items)
    {
        foreach (CalendarEvent item in items)
        {
            await DeleteAsync(id:item.Id);
        }
    }
}