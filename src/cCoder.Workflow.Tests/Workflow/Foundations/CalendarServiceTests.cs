// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Brokers.Storage;
using cCoder.Workflow.Services.Foundations;
using Moq;

namespace cCoder.Core.Services.Tests.Workflow.Foundations;

#pragma warning disable STXFORMAT008
public partial class CalendarServiceTests
{
    private readonly Mock<ICalendarBroker> calendarBrokerMock = new();
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();
    private readonly CalendarService calendarService;

    public CalendarServiceTests()
    {

        calendarService = new CalendarService(
            calendarBroker: calendarBrokerMock.Object,
            authorizationBroker: authorizationBrokerMock.Object);

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