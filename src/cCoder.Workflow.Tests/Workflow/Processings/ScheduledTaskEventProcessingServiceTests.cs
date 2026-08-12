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

public partial class ScheduledTaskEventProcessingServiceTests
{
    private readonly Mock<IScheduledTaskEventService> scheduledTaskEventServiceMock;
    private readonly ScheduledTaskEventProcessingService service;

    public ScheduledTaskEventProcessingServiceTests()
    {
        scheduledTaskEventServiceMock = new Mock<IScheduledTaskEventService>(behavior: MockBehavior.Strict);
        service = new ScheduledTaskEventProcessingService(scheduledTaskEventServiceMock.Object);
    }

    private static ScheduledTask CreateRandomScheduledTask() =>
        Builder<ScheduledTask>.CreateNew()
            .Build();
}