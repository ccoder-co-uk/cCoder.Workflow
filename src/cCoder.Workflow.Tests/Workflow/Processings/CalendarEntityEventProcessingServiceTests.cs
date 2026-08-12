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

public partial class CalendarEntityEventProcessingServiceTests
{
    private readonly Mock<ICalendarEntityEventService> calendarEntityEventServiceMock;
    private readonly CalendarEntityEventProcessingService service;

    public CalendarEntityEventProcessingServiceTests()
    {
        calendarEntityEventServiceMock = new Mock<ICalendarEntityEventService>(behavior: MockBehavior.Strict);
        service = new CalendarEntityEventProcessingService(calendarEntityEventServiceMock.Object);
    }

    private static Calendar CreateRandomCalendar() =>
        Builder<Calendar>.CreateNew()
            .Build();
}