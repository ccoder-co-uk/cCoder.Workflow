// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using System.Net;
using Xunit;

namespace Web.AcceptanceTests.Tests.Api;

public sealed partial class HomePageTests
{
    [Fact]
    public async Task Get_RedirectsToWorkflowTester()
    {
        // Given

        // When
        using HttpResponseMessage response = await GetHomeAsync();

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location?.OriginalString.Should().Be("/tools/index.html");
    }

    [Fact]
    public async Task GetTools_ReturnsWorkflowTester()
    {
        // Given

        // When
        string actualHome = await GetToolsAsync();

        // Then
        actualHome.Should().Contain("<title>Workflow</title>");
        actualHome.Should().Contain("/tools/company-logo.png");
        actualHome.Should().Contain("wf-logo");
        actualHome.Should().Contain("Sign in required");
        actualHome.Should().Contain("wf-login-gate");
        actualHome.Should().Contain("wf-workbench");
        actualHome.Should().Contain("Workflow workspace tabs");
        actualHome.Should().Contain("/tools/api.js");
        actualHome.Should().Contain("/tools/grids.js");
        actualHome.Should().Contain("Flow definition and execution management");
    }

    [Fact]
    public async Task GetToolsApi_ReturnsLoginGateLogic()
    {
        // Given

        // When
        string actualScript = await Client.GetStringAsync("/tools/api.js");

        // Then
        actualScript.Should().Contain("workflow-auth-changed");
        actualScript.Should().Contain("isAuthenticated: function");
        actualScript.Should().Contain("document.body.classList.toggle(\"is-authenticated\"");
    }

    [Fact]
    public async Task GetToolsScripts_ReturnsAuthenticatedGridStartupLogic()
    {
        // Given

        // When
        string actualScript = await Client.GetStringAsync("/tools/grids.js");

        // Then
        actualScript.Should().Contain("WorkflowApi.isAuthenticated()");
        actualScript.Should().Contain("workflow-auth-changed");
    }

    [Fact]
    public async Task GetToolsStyles_ReturnsLoginGateStyles()
    {
        // Given

        // When
        string actualStyles = await Client.GetStringAsync("/tools/styles.css");

        // Then
        actualStyles.Should().Contain("body.wf-shell:not(.is-authenticated) .wf-workbench");
        actualStyles.Should().Contain("body.wf-shell.is-authenticated .wf-login-gate");
        actualStyles.Should().Contain(".wf-logo");
        actualStyles.Should().Contain("grid-template-rows: auto minmax(0, 1fr)");
        actualStyles.Should().Contain(".wf-nav-item.active");
    }
}