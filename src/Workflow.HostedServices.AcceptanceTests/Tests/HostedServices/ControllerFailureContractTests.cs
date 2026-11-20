// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT001, STXFORMAT002, STXFORMAT005, STXFORMAT009, STXFORMAT013, STXTEST005

using System;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Workflow.HostedServices.Brokers.Loggings;
using Workflow.HostedServices.Controllers;
using Workflow.HostedServices.Exposures;
using Xunit;

namespace Web.AcceptanceTests.Tests.HostedServices;

public sealed partial class ControllerFailureContractTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Controller_WhenValidationFails_Returns400AndLogsWarning(bool health)
    {
        var exception = new cCoder.Workflow.Models.Exceptions.WorkflowValidationException(innerException: new Exception("invalid"));
        var logging = new RecordingLoggingBroker();

        IActionResult result = health
            ? new HealthController(new HealthManager(exception), logging).Get()
            : new HomeController(new HomeManager(exception), logging).Get();

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        logging.WarningCount.Should().Be(1);
        logging.ErrorCount.Should().Be(0);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Controller_WhenUnexpectedFailureOccurs_Returns500AndLogsError(bool health)
    {
        var exception = new InvalidOperationException("database unavailable");
        var logging = new RecordingLoggingBroker();

        IActionResult result = health
            ? new HealthController(new HealthManager(exception), logging).Get()
            : new HomeController(new HomeManager(exception), logging).Get();

        result.Should().BeAssignableTo<IStatusCodeActionResult>().Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        logging.WarningCount.Should().Be(0);
        logging.ErrorCount.Should().Be(1);
    }

    sealed class HealthManager(Exception exception) : IHealthManager
    {
        public string GetHealth() => throw exception;
    }

    sealed class HomeManager(Exception exception) : IHomeManager
    {
        public string GetHome() => throw exception;
    }

    sealed class RecordingLoggingBroker : ILoggingBroker
    {
        public int WarningCount { get; private set; }
        public int ErrorCount { get; private set; }
        public void LogWarning(Exception exception, string message) => WarningCount++;

        public void LogError(Exception exception, string message) => ErrorCount++;
    }
}

#pragma warning restore STXFORMAT001, STXFORMAT002, STXFORMAT005, STXFORMAT009, STXFORMAT013, STXTEST005