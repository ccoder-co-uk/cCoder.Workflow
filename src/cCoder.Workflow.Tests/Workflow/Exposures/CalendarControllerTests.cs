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
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Exposures;

public partial class CalendarControllerTests
{
    private readonly Mock<ICalendarManager> calendarManagerMock = new();
    private readonly Mock<ILoggingBroker> loggingBrokerMock = new();
    private readonly CalendarController controller;

    public static TheoryData<Exception, int> FailureExceptions => new()
    {
        { new cCoder.Workflow.Models.Exceptions.WorkflowValidationException(innerException: new Exception()), 400 },
        { new System.Security.SecurityException(), 403 },
        { new Exception(), 500 }
    };

    public CalendarControllerTests()
    {
        controller = new CalendarController(
            service: calendarManagerMock.Object,
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