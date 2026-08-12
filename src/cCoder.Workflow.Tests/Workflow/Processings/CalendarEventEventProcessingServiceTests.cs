// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Planning;
using cCoder.Workflow.Services.Foundations.Events;
using cCoder.Workflow.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class CalendarEventEventProcessingServiceTests
{
    private readonly Mock<ICalendarEventEventService> calendarEventEventServiceMock;
    private readonly CalendarEventEventProcessingService service;

    public CalendarEventEventProcessingServiceTests()
    {
        calendarEventEventServiceMock = new Mock<ICalendarEventEventService>(behavior: MockBehavior.Strict);
        service = new CalendarEventEventProcessingService(calendarEventEventServiceMock.Object);
    }

    private static CalendarEvent CreateRandomCalendarEvent() =>
        Builder<CalendarEvent>.CreateNew()
            .Build();
}