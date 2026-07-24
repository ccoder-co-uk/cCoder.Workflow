// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers;
using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations;
using cCoder.Workflow.Services.Processings;
using FizzWare.NBuilder;
using Microsoft.Extensions.Logging;
using Moq;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class FlowDefinitionProcessingServiceTests
{
    private readonly Mock<IFlowDefinitionService> flowDefinitionServiceMock = new();
    private readonly Mock<IJsonBroker> jsonBrokerMock = new();
    private readonly Mock<ILogger<FlowDefinitionProcessingService>> loggerMock = new();
    private readonly FlowDefinitionProcessingService flowDefinitionProcessingService;

    public FlowDefinitionProcessingServiceTests()
    {
        flowDefinitionProcessingService = new FlowDefinitionProcessingService(
            flowDefinitionServiceMock.Object,
            jsonBrokerMock.Object,
            loggerMock.Object
        );
    }

    private static FlowDefinition CreateRandomFlowDefinition() =>
        Builder<FlowDefinition>
            .CreateNew()
            .With(func:x => x.Id = Guid.NewGuid())
            .With(func:x => x.AppId = 1)
            .With(func:x => x.Name = $"Flow-{Guid.NewGuid():N}")
            .With(func:x => x.DefinitionJson = "{}")
            .With(func:x => x.ConfigJson = "{}")
            .With(func:x => x.App = null)
            .With(func:x => x.Instances = [])
            .Build();
}