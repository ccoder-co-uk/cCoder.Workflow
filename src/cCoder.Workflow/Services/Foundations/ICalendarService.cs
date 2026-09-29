// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Services.Foundations;

internal interface ICalendarService
{
    object CreateSingleResult<T>(IQueryable<T> queryable);

    Calendar Get(int calendarId);

    IQueryable<Calendar> GetAll(bool ignoreFilters = false);

    ValueTask<Calendar> AddCalendarAsync(Calendar newCalendar);

    ValueTask<Calendar> UpdateCalendarAsync(Calendar updatedCalendar);

    ValueTask DeleteAsync(int calendarId);

    ValueTask DeleteAllForAppCalendarAsync(IEnumerable<Calendar> deletedItems);

    ValueTask DeleteAllByAppIdAsync(int appId);
}