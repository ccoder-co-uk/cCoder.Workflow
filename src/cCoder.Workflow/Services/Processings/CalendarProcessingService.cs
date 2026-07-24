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

internal sealed partial class CalendarProcessingService(ICalendarService service, ICalendarEventProcessingService calendarEventService, IAuthorizationBroker authorizationBroker) : ICalendarProcessingService
{
    public Calendar Get(int calendarId) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [calendarId]); return ExecuteGet(calendarId: calendarId); });

    private Calendar ExecuteGet(int calendarId)
    {
        return service.Get(calendarId: calendarId);
    }

    public IQueryable<Calendar> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: () => { ValidateInputs(inputs: [ignoreFilters]); return ExecuteGetAll(ignoreFilters: ignoreFilters); });

    private IQueryable<Calendar> ExecuteGetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask<Calendar> AddCalendarAsync(Calendar newEntity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [newEntity]); return await ExecuteAddAsync(entity: newEntity); }, isValueTask: true);

    private ValueTask<Calendar> ExecuteAddAsync(Calendar entity)
    {
        return service.AddCalendarAsync(newCalendar: entity);
    }

    public ValueTask<Calendar> UpdateCalendarAsync(Calendar updatedEntity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [updatedEntity]); return await ExecuteUpdateAsync(entity: updatedEntity); }, isValueTask: true);

    private ValueTask<Calendar> ExecuteUpdateAsync(Calendar entity)
    {
        return service.UpdateCalendarAsync(updatedCalendar: entity);
    }

    public ValueTask DeleteAsync(int calendarId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [calendarId]); await ExecuteDeleteAsync(calendarId: calendarId); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAsync(int calendarId)
    {
        Calendar calendar = Get(calendarId: calendarId);
        authorizationBroker.Authorize(appId: calendar.AppId, privilege: "calendar_delete");

        CalendarEvent[] events = (from ce in calendarEventService.GetAll()
                                  where ce.CalendarId == calendar.Id
                                  select ce).ToArray();

        await calendarEventService.DeleteAllCalendarEventAsync(deletedItems: events);
        await service.DeleteAsync(calendarId: calendar.Id);
    }

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [appId]); await ExecuteDeleteByAppIdAsync(appId: appId); }, isValueTask: true);

    private async ValueTask ExecuteDeleteByAppIdAsync(int appId)
    {
        await calendarEventService.DeleteAllByAppIdAsync(appId: appId);
        await service.DeleteAllByAppIdAsync(appId: appId);
    }

    public ValueTask<IEnumerable<Result<Calendar>>> AddOrUpdateCalendar(IEnumerable<Calendar> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); return await ExecuteAddOrUpdate(items: items); }, isValueTask: true);

    private async ValueTask<IEnumerable<Result<Calendar>>> ExecuteAddOrUpdate(IEnumerable<Calendar> items)
    {
        List<Result<Calendar>> results = new List<Result<Calendar>>();

        foreach (Calendar item in items)
        {
            try
            {
                Calendar savedItem =
                    item.Id == 0
                        ? await AddCalendarAsync(newEntity: item)
                        : await UpdateCalendarAsync(updatedEntity: item);

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

    public ValueTask DeleteAllCalendarAsync(IEnumerable<Calendar> deletedItems) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [deletedItems]); await ExecuteDeleteAllAsync(items: deletedItems); }, isValueTask: true);

    private async ValueTask ExecuteDeleteAllAsync(IEnumerable<Calendar> items)
    {
        foreach (Calendar item in items)
        {
            await DeleteAsync(calendarId: item.Id);
        }
    }
}