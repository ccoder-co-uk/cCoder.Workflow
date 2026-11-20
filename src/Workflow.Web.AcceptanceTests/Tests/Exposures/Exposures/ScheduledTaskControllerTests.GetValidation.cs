// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT001, STXFORMAT005, STXFORMAT008, STXFORMAT009, STXFORMAT013, STXTEST005

using System;
using cCoder.Workflow.Models.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Exposures;

public partial class ScheduledTaskControllerTests
{
    [Fact]
    public void Get_WhenValidationFails_Returns400AndLogsWarning()
    {
        var exception = new WorkflowValidationException(innerException: new Exception("invalid"));
        scheduledTaskManagerMock.Setup(service => service.GetAll(false)).Throws(exception);

        IActionResult result = controller.Get(key: 1);

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        loggingBrokerMock.Verify(
            broker => broker.LogWarning(exception, It.IsAny<string>(), It.IsAny<object[]>()),
            Times.Once);
        loggingBrokerMock.Verify(
            broker => broker.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()),
            Times.Never);
        scheduledTaskManagerMock.Verify(service => service.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Never);
    }
}

#pragma warning restore STXFORMAT001, STXFORMAT005, STXFORMAT008, STXFORMAT009, STXFORMAT013, STXTEST005