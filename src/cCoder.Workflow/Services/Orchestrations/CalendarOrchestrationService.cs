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

internal sealed partial class CalendarOrchestrationService(ICalendarProcessingService processingService, ICalendarEntityEventProcessingService eventService) : ICalendarOrchestrationService
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

    public ValueTask<Calendar> AddAsync(Calendar entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); return await ExecuteAddAsync(entity: entity); }, isValueTask: true);

    private async ValueTask<Calendar> ExecuteAddAsync(Calendar entity)
    {
        Calendar result = await processingService.AddAsync(entity: entity);
        await eventService.RaiseCalendarAddEventAsync(entity: result);
        return result;
    }

    public ValueTask<Calendar> UpdateAsync(Calendar entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); return await ExecuteUpdateAsync(entity: entity); }, isValueTask: true);

    private async ValueTask<Calendar> ExecuteUpdateAsync(Calendar entity)
    {
        Calendar result = await processingService.UpdateAsync(entity: entity);
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

        await eventService.RaiseCalendarDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(calendarId: calendarId);
    }

    public ValueTask DeleteByAppIdAsync(int appId) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [appId]); await ExecuteDeleteByAppIdAsync(appId: appId); }, isValueTask: true);

    private ValueTask ExecuteDeleteByAppIdAsync(int appId) =>
        processingService.DeleteByAppIdAsync(appId: appId);

    public ValueTask<IEnumerable<Result<Calendar>>> AddOrUpdate(IEnumerable<Calendar> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); return await ExecuteAddOrUpdate(items: items); }, isValueTask: true);

    private ValueTask<IEnumerable<Result<Calendar>>> ExecuteAddOrUpdate(IEnumerable<Calendar> items)
    {
        return processingService.AddOrUpdate(items: items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<Calendar> items) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [items]); await ExecuteDeleteAllAsync(items: items); }, isValueTask: true);

    private ValueTask ExecuteDeleteAllAsync(IEnumerable<Calendar> items)
    {
        return processingService.DeleteAllAsync(items: items);
    }
}