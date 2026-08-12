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

public partial class CalendarEventOrchestrationServiceTests
{
    private readonly Mock<ICalendarEventProcessingService> calendarEventProcessingServiceMock;
    private readonly Mock<ICalendarEventEventProcessingService> calendarEventEventProcessingServiceMock;
    private readonly CalendarEventOrchestrationService orchestrationService;

    public CalendarEventOrchestrationServiceTests()
    {
        calendarEventProcessingServiceMock = new Mock<ICalendarEventProcessingService>(behavior: MockBehavior.Strict);
        calendarEventEventProcessingServiceMock = new Mock<ICalendarEventEventProcessingService>(behavior: MockBehavior.Strict);
        orchestrationService = new CalendarEventOrchestrationService(
            calendarEventProcessingServiceMock.Object,
            calendarEventEventProcessingServiceMock.Object
        );
    }

    private static CalendarEvent CreateRandomCalendarEvent() =>
        Builder<CalendarEvent>.CreateNew()
            .Build();
}