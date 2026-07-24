// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations.Events;


namespace cCoder.Workflow.Services.Processings;

internal class CalendarEntityEventProcessingService(ICalendarEntityEventService eventService) : ICalendarEntityEventProcessingService
{
    public ValueTask RaiseCalendarAddEventAsync(Calendar entity) => eventService.RaiseCalendarAddEventAsync(entity: entity);

    public ValueTask RaiseCalendarUpdateEventAsync(Calendar entity) => eventService.RaiseCalendarUpdateEventAsync(entity: entity);

    public ValueTask RaiseCalendarDeleteEventAsync(Calendar entity) => eventService.RaiseCalendarDeleteEventAsync(entity: entity);
}