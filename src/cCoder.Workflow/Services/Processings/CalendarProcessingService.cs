// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers;
using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations;

namespace cCoder.Workflow.Services.Processings;

internal class CalendarProcessingService(ICalendarService service, ICalendarEventProcessingService calendarEventService, IAuthorizationBroker authorizationBroker) : ICalendarProcessingService
{
    public Calendar Get(int calendarId)
    {
        return service.Get(calendarId: calendarId);
    }

    public IQueryable<Calendar> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask<Calendar> AddAsync(Calendar entity)
    {
        return service.AddAsync(calendar: entity);
    }

    public ValueTask<Calendar> UpdateAsync(Calendar entity)
    {
        return service.UpdateAsync(calendar: entity);
    }

    public async ValueTask DeleteAsync(int calendarId)
    {
        Calendar calendar = Get(calendarId: calendarId);
        authorizationBroker.Authorize(appId: calendar.AppId, privilege: "calendar_delete");
        CalendarEvent[] events = (from ce in calendarEventService.GetAll()
                                  where ce.CalendarId == calendar.Id
                                  select ce).ToArray();
        await calendarEventService.DeleteAllAsync(items: events);
        await service.DeleteAsync(calendarId: calendar.Id);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        await calendarEventService.DeleteAllByAppIdAsync(appId: appId);
        await service.DeleteAllByAppIdAsync(appId: appId);
    }

    public async ValueTask<IEnumerable<Result<Calendar>>> AddOrUpdate(IEnumerable<Calendar> items)
    {
        List<Result<Calendar>> results = new List<Result<Calendar>>();

        foreach (Calendar item in items)
        {
            try
            {
                Calendar savedItem =
                    item.Id == 0
                        ? await AddAsync(entity: item)
                        : await UpdateAsync(entity: item);

                results.Add(item: new Result<Calendar>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<Calendar>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<Calendar> items)
    {
        foreach (Calendar item in items)
        {
            await DeleteAsync(calendarId: item.Id);
        }
    }
}