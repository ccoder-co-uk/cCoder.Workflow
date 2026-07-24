// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations;
using cCoder.Workflow.Services.Processings;
using FizzWare.NBuilder;
using Microsoft.Extensions.Logging;
using Moq;
using IAuthorizationBroker = cCoder.Workflow.Brokers.IAuthorizationBroker;
using IJsonBroker = cCoder.Workflow.Brokers.IJsonBroker;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class WorkflowEventProcessingServiceTests
{
    private readonly Mock<IWorkflowEventService> workflowEventServiceMock = new();
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();
    private readonly Mock<IJsonBroker> jsonBrokerMock = new();
    private readonly Mock<ILogger<WorkflowEventProcessingService>> loggerMock = new();
    private readonly WorkflowEventProcessingService workflowEventProcessingService;

    public WorkflowEventProcessingServiceTests()
    {
        workflowEventProcessingService = new WorkflowEventProcessingService(
            workflowEventServiceMock.Object,
            authorizationBrokerMock.Object,
            jsonBrokerMock.Object,
            loggerMock.Object);
    }

    private static WorkflowEvent CreateRandomWorkflowEvent() =>
        Builder<WorkflowEvent>
            .CreateNew()
            .With(func: x => x.Id = Guid.NewGuid())
            .With(func: x => x.FlowId = Guid.NewGuid())
            .With(func: x => x.ExecuteAs = Guid.NewGuid()
            .ToString())
            .Build();
}