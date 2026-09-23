// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Brokers.Loggings;
using cCoder.Workflow.Engine.Services.Foundations;
using cCoder.Workflow.Engine.Services.Processings;
using Moq;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class WorkflowScriptExecutionFoundationServiceTests
{
    private readonly Mock<IScriptService> scriptBrokerMock =
        new(behavior: MockBehavior.Strict);

    private readonly Mock<ILoggingBroker> loggingBrokerMock = new();

    private WorkflowScriptExecutionProcessingService CreateService() =>
        new(
            scriptService: scriptBrokerMock.Object,
            jsonBroker: new JsonBroker(),
            logger: loggingBrokerMock.Object);
}