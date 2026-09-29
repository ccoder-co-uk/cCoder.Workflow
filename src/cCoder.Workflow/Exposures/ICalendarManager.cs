// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Workflow.Models.Results;
using cCoder.Data.Models.Planning;

namespace cCoder.Workflow.Exposures;

public interface ICalendarManager : IControllerErrorLogger
{
    Calendar Get(int calendarId);

    IQueryable<Calendar> GetAll(bool ignoreFilters = false);

    ValueTask<Calendar> AddCalendarAsync(Calendar newCalendar);

    ValueTask<Calendar> UpdateCalendarAsync(Calendar updatedCalendar);

    ValueTask DeleteAsync(int calendarId);

    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<Result<Calendar>>> AddOrUpdateCalendar(IEnumerable<Calendar> items);

    ValueTask DeleteAllCalendarAsync(IEnumerable<Calendar> deletedItems);
}