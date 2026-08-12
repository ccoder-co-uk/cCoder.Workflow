// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Brokers.Loggings;
using cCoder.Workflow.Engine.Services.Processings;
using Moq;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class WorkflowScriptExecutionProcessingServiceTests
{
    private readonly Mock<IScriptBroker> scriptBrokerMock =
        new(behavior: MockBehavior.Strict);

    private readonly Mock<ILoggingBroker> loggingBrokerMock = new();

    private WorkflowScriptExecutionProcessingService CreateService() =>
        new(
            scriptBroker: scriptBrokerMock.Object,
            logger: loggingBrokerMock.Object);
}