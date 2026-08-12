// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Planning;
using cCoder.Workflow.Services.Orchestrations;
using cCoder.Workflow.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class CalendarOrchestrationServiceTests
{
    private readonly Mock<ICalendarProcessingService> calendarProcessingServiceMock;
    private readonly Mock<ICalendarEventProcessingService> calendarEventProcessingServiceMock;
    private readonly Mock<ICalendarEntityEventProcessingService> eventServiceMock;
    private readonly CalendarOrchestrationService orchestrationService;

    public CalendarOrchestrationServiceTests()
    {
        calendarProcessingServiceMock = new Mock<ICalendarProcessingService>(behavior: MockBehavior.Strict);
        calendarEventProcessingServiceMock = new Mock<ICalendarEventProcessingService>(behavior: MockBehavior.Strict);
        eventServiceMock = new Mock<ICalendarEntityEventProcessingService>(behavior: MockBehavior.Strict);
        orchestrationService = new CalendarOrchestrationService(
            processingService: calendarProcessingServiceMock.Object,
            calendarEventProcessingService: calendarEventProcessingServiceMock.Object,
            eventService: eventServiceMock.Object);
    }

    private static Calendar CreateRandomCalendar() =>
        Builder<Calendar>.CreateNew()
            .Build();
}