// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Workflow.Services.Foundations;
using cCoder.Workflow.Services.Processings;
using Moq;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

#pragma warning disable STXFORMAT008
public partial class CalendarProcessingServiceTests
{
    private readonly Mock<ICalendarService> calendarServiceMock = new();

    private readonly CalendarProcessingService processingService;

    public CalendarProcessingServiceTests()
    {
        processingService = new CalendarProcessingService(
            service: calendarServiceMock.Object);
    }

    private static Calendar CreateCalendar() =>
        new()
        {
            Id = Random.Shared.Next(minValue: 1, maxValue: int.MaxValue),
            AppId = Random.Shared.Next(minValue: 1, maxValue: int.MaxValue),
            Name = "Calendar",
            Description = "Description"
        };
}
#pragma warning restore STXFORMAT008