// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT001, STXFORMAT005, STXFORMAT008, STXFORMAT009, STXFORMAT013, STXTEST005

using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Exposures;

public partial class FlowDefinitionControllerTests
{
    [Fact]
    public async Task PostScript_WhenExecutionSucceeds_ReturnsOk()
    {
        byte[] body = Encoding.UTF8.GetBytes("return true;");
        controller.Request.Body = new MemoryStream(body);
        flowDefinitionManagerMock.Setup(service => service.ReadRequestBodyAsync(It.IsAny<Stream>())).ReturnsAsync("return true;");
        flowDefinitionManagerMock.Setup(service => service.ExecuteScriptAsync("return true;")).ReturnsAsync("true");

        IActionResult result = await controller.PostScript();

        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be("true");
        loggingBrokerMock.Verify(
            broker => broker.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()),
            Times.Once);
        loggingBrokerMock.Verify(
            broker => broker.LogWarning(It.IsAny<System.Exception>(), It.IsAny<string>(), It.IsAny<object[]>()),
            Times.Never);
        loggingBrokerMock.Verify(
            broker => broker.LogError(It.IsAny<System.Exception>(), It.IsAny<string>(), It.IsAny<object[]>()),
            Times.Never);
    }
}

#pragma warning restore STXFORMAT001, STXFORMAT005, STXFORMAT008, STXFORMAT009, STXFORMAT013, STXTEST005