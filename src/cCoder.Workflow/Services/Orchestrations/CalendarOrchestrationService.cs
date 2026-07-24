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

internal class CalendarOrchestrationService(ICalendarProcessingService processingService, ICalendarEntityEventProcessingService eventService) : ICalendarOrchestrationService
{
    public Calendar Get(int id)
    {
        return processingService.Get(id);
    }

    public IQueryable<Calendar> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters);
    }

    public async ValueTask<Calendar> AddAsync(Calendar entity)
    {
        Calendar result = await processingService.AddAsync(entity);
        await eventService.RaiseCalendarAddEventAsync(result);
        return result;
    }

    public async ValueTask<Calendar> UpdateAsync(Calendar entity)
    {
        Calendar result = await processingService.UpdateAsync(entity);
        await eventService.RaiseCalendarUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        Calendar entity = processingService.GetAll(ignoreFilters: true).FirstOrDefault(item => item.Id == id);

        if (entity is null)
            return;

        await eventService.RaiseCalendarDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public ValueTask DeleteByAppIdAsync(int appId) =>
        processingService.DeleteByAppIdAsync(appId);

    public ValueTask<IEnumerable<Result<Calendar>>> AddOrUpdate(IEnumerable<Calendar> items)
    {
        return processingService.AddOrUpdate(items);
    }

    public ValueTask DeleteAllAsync(IEnumerable<Calendar> items)
    {
        return processingService.DeleteAllAsync(items);
    }
}