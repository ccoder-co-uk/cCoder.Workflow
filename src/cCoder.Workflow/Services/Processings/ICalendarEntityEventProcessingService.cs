// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Processings;

internal interface ICalendarEntityEventProcessingService
{
    ValueTask RaiseCalendarAddEventAsync(Calendar calendar);

    ValueTask RaiseCalendarUpdateEventAsync(Calendar calendar);

    ValueTask RaiseCalendarDeleteEventAsync(Calendar calendar);
}