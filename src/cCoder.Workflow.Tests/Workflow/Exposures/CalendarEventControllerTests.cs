// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Workflow.Brokers.Loggings;
using cCoder.Workflow.Exposures.Controllers;
using cCoder.Workflow.Exposures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace cCoder.Core.Services.Tests.Workflow.Exposures;

public partial class CalendarEventControllerTests
{
    private readonly Mock<ICalendarEventManager> calendarEventManagerMock = new();
    private readonly Mock<ILoggingBroker> loggingBrokerMock = new();
    private readonly CalendarEventController controller;

    public CalendarEventControllerTests()
    {
        controller = new CalendarEventController(
            service: calendarEventManagerMock.Object,
            loggingBroker: loggingBrokerMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005