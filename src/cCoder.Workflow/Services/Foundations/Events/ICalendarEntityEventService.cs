// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Services.Foundations.Events;

internal interface ICalendarEntityEventService
{
    ValueTask RaiseCalendarAddEventAsync(Calendar calendar);

    ValueTask RaiseCalendarUpdateEventAsync(Calendar calendar);

    ValueTask RaiseCalendarDeleteEventAsync(Calendar calendar);
}