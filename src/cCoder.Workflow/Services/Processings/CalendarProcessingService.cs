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
    public Calendar Get(int id)
    {
        return service.Get(id);
    }

    public IQueryable<Calendar> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

    public ValueTask<Calendar> AddAsync(Calendar entity)
    {
        return service.AddAsync(entity);
    }

    public ValueTask<Calendar> UpdateAsync(Calendar entity)
    {
        return service.UpdateAsync(entity);
    }

    public async ValueTask DeleteAsync(int id)
    {
        Calendar calendar = Get(id);
        authorizationBroker.Authorize(calendar.AppId, "calendar_delete");
        CalendarEvent[] events = (from ce in calendarEventService.GetAll()
                                  where ce.CalendarId == calendar.Id
                                  select ce).ToArray();
        await calendarEventService.DeleteAllAsync(events);
        await service.DeleteAsync(calendar.Id);
    }

    public async ValueTask DeleteByAppIdAsync(int appId)
    {
        Calendar[] calendars =
            [.. service.GetAll(ignoreFilters: true)
                .Where(calendar => calendar.AppId == appId)];

        if (calendars.Length == 0)
        {
            return;
        }

        int[] calendarIds = [.. calendars.Select(calendar => calendar.Id)];
        CalendarEvent[] events =
            [.. calendarEventService.GetAll(ignoreFilters: true)
                .Where(calendarEvent => calendarIds.Contains(calendarEvent.CalendarId))];

        await calendarEventService.DeleteAllForAppAsync(events);
        await service.DeleteAllForAppAsync(calendars);
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
                        ? await AddAsync(item)
                        : await UpdateAsync(item);

                results.Add(new Result<Calendar>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result<Calendar>
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
            await DeleteAsync(item.Id);
        }
    }
}
