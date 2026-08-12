// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005

using cCoder.Workflow.Brokers.Loggings;
using cCoder.Workflow.Exposures.Controllers;
using cCoder.Workflow.Exposures;
using cCoder.Security.Models.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace cCoder.Core.Services.Tests.Workflow.Exposures;

public partial class FlowDefinitionControllerTests
{
    private readonly Mock<IFlowDefinitionManager> flowDefinitionManagerMock = new();
    private readonly Mock<ILoggingBroker> loggingBrokerMock = new();
    private readonly FlowDefinitionController controller;

    public FlowDefinitionControllerTests()
    {
        controller = new FlowDefinitionController(
            service: flowDefinitionManagerMock.Object,
            authInfo: Mock.Of<ISSOAuthInfo>(),
            loggingBroker: loggingBrokerMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }
}

#pragma warning restore STXFORMAT005, STXFORMAT008, STXFORMAT009, STXTEST005