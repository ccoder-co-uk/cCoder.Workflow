// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Services.Processings;

internal interface ICalendarEntityEventProcessingService
{
    ValueTask RaiseCalendarAddEventAsync(Calendar calendar);

    ValueTask RaiseCalendarUpdateEventAsync(Calendar calendar);

    ValueTask RaiseCalendarDeleteEventAsync(Calendar calendar);
}