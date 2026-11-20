// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT001, STXFORMAT005, STXFORMAT008, STXFORMAT009, STXFORMAT013, STXTEST005

using System;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Workflow.Models.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using Workflow.Web.Brokers.Loggings;
using Workflow.Web.Controllers;
using Workflow.Web.Exposures;
using Xunit;

namespace Web.AcceptanceTests.Tests.Api;

public sealed partial class CoreControllerContractTests
{
    [Fact]
    public void CoreUserGet_WhenUserExists_ReturnsOk()
    {
        var managerMock = new Mock<ICoreUserManager>();
        var loggingMock = new Mock<ILoggingBroker>();
        managerMock.Setup(manager => manager.GetUser()).Returns(new User());
        var controller = new CoreUserController(managerMock.Object, loggingMock.Object);

        IActionResult result = controller.Get();

        result.Should().BeOfType<OkObjectResult>();
        loggingMock.Verify(broker => broker.LogInformation(It.IsAny<string>()), Times.Once);
        loggingMock.Verify(broker => broker.LogWarning(It.IsAny<Exception>(), It.IsAny<string>()), Times.Never);
        loggingMock.Verify(broker => broker.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CoreAppGet_WhenAppExists_ReturnsOk()
    {
        var managerMock = new Mock<ICoreAppManager>();
        var loggingMock = new Mock<ILoggingBroker>();
        managerMock.Setup(manager => manager.GetAppAsync(1)).ReturnsAsync(new App { Id = 1 });
        var controller = new CoreAppController(managerMock.Object, loggingMock.Object);

        IActionResult result = await controller.Get(key: 1);

        result.Should().BeOfType<OkObjectResult>();
        loggingMock.Verify(broker => broker.LogInformation(It.IsAny<string>()), Times.Once);
        loggingMock.Verify(broker => broker.LogWarning(It.IsAny<Exception>(), It.IsAny<string>()), Times.Never);
        loggingMock.Verify(broker => broker.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData("Health")]
    [InlineData("User")]
    [InlineData("App")]
    public async Task Controller_WhenValidationFails_Returns400AndLogsWarning(string controllerName)
    {
        var exception = new WorkflowValidationException(innerException: new Exception("invalid"));
        var loggingMock = new Mock<ILoggingBroker>();

        IActionResult result = await InvokeFailure(controllerName, exception, loggingMock);

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        loggingMock.Verify(broker => broker.LogInformation(It.IsAny<string>()), Times.Never);
        loggingMock.Verify(broker => broker.LogWarning(exception, It.IsAny<string>()), Times.Once);
        loggingMock.Verify(broker => broker.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData("Health")]
    [InlineData("User")]
    [InlineData("App")]
    public async Task Controller_WhenUnexpectedFailureOccurs_Returns500AndLogsError(string controllerName)
    {
        var exception = new InvalidOperationException("database unavailable");
        var loggingMock = new Mock<ILoggingBroker>();

        IActionResult result = await InvokeFailure(controllerName, exception, loggingMock);

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        loggingMock.Verify(broker => broker.LogInformation(It.IsAny<string>()), Times.Never);
        loggingMock.Verify(broker => broker.LogWarning(It.IsAny<Exception>(), It.IsAny<string>()), Times.Never);
        loggingMock.Verify(broker => broker.LogError(exception, It.IsAny<string>()), Times.Once);
    }

    private static async Task<IActionResult> InvokeFailure(
        string controllerName,
        Exception exception,
        Mock<ILoggingBroker> loggingMock)
    {
        if (controllerName == "Health")
        {
            var managerMock = new Mock<IHealthManager>();
            managerMock.Setup(manager => manager.GetHealth()).Throws(exception);
            return new HealthController(managerMock.Object, loggingMock.Object).Get();
        }

        if (controllerName == "User")
        {
            var managerMock = new Mock<ICoreUserManager>();
            managerMock.Setup(manager => manager.GetUser()).Throws(exception);
            return new CoreUserController(managerMock.Object, loggingMock.Object).Get();
        }

        var appManagerMock = new Mock<ICoreAppManager>();
        appManagerMock.Setup(manager => manager.GetAppAsync(1)).Throws(exception);
        return await new CoreAppController(appManagerMock.Object, loggingMock.Object).Get(key: 1);
    }
}

#pragma warning restore STXFORMAT001, STXFORMAT005, STXFORMAT008, STXFORMAT009, STXFORMAT013, STXTEST005