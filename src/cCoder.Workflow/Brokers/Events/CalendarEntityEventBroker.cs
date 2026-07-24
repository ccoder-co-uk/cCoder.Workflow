// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Eventing.Models;
using cCoder.Data;
using cCoder.Eventing;
using Microsoft.Extensions.DependencyInjection;


namespace cCoder.Workflow.Brokers.Events;

internal sealed class CalendarEntityEventBroker(
    IServiceProvider serviceProvider)
        : ICalendarEntityEventBroker
{
    public string GetCurrentUserId() =>
        serviceProvider.GetRequiredService<ICoreAuthInfo>().SSOUserId;

    public ValueTask RaiseCalendarAddEventAsync(EventMessage<Calendar> message) =>
        serviceProvider.GetRequiredService<IEventHub>()
            .RaiseEventAsync(name: "calendar_add", message: message);

    public ValueTask RaiseCalendarUpdateEventAsync(EventMessage<Calendar> message) =>
        serviceProvider.GetRequiredService<IEventHub>()
            .RaiseEventAsync(name: "calendar_update", message: message);

    public ValueTask RaiseCalendarDeleteEventAsync(EventMessage<Calendar> message) =>
        serviceProvider.GetRequiredService<IEventHub>()
            .RaiseEventAsync(name: "calendar_delete", message: message);
}