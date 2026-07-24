// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Services.Orchestrations;

public interface ICalendarOrchestrationService
{
    Calendar Get(int calendarId);

    IQueryable<Calendar> GetAll(bool ignoreFilters = false);

    ValueTask<Calendar> AddCalendarAsync(Calendar newEntity);

    ValueTask<Calendar> UpdateCalendarAsync(Calendar updatedEntity);

    ValueTask DeleteAsync(int calendarId);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<Result<Calendar>>> AddOrUpdateCalendar(IEnumerable<Calendar> items);

    ValueTask DeleteAllCalendarAsync(IEnumerable<Calendar> deletedItems);
}