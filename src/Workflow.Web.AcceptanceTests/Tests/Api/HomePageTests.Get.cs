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
        response.StatusCode.Should().Be(expected:HttpStatusCode.Redirect);
        response.Headers.Location?.OriginalString.Should().Be(expected:"/tools/index.html");
    }

    [Fact]
    public async Task GetTools_ReturnsWorkflowTester()
    {
        // Given

        // When
        string actualHome = await GetToolsAsync();

        // Then
        actualHome.Should().Contain(expected:"<title>Workflow</title>");
        actualHome.Should().Contain(expected:"/tools/company-logo.png");
        actualHome.Should().Contain(expected:"wf-logo");
        actualHome.Should().Contain(expected:"Sign in required");
        actualHome.Should().Contain(expected:"wf-login-gate");
        actualHome.Should().Contain(expected:"wf-workbench");
        actualHome.Should().Contain(expected:"Workflow workspace tabs");
        actualHome.Should().Contain(expected:"/tools/api.js");
        actualHome.Should().Contain(expected:"/tools/grids.js");
        actualHome.Should().Contain(expected:"Flow definition and execution management");
    }

    [Fact]
    public async Task GetToolsApi_ReturnsLoginGateLogic()
    {
        // Given

        // When
        string actualScript = await Client.GetStringAsync(requestUri:"/tools/api.js");

        // Then
        actualScript.Should().Contain(expected:"workflow-auth-changed");
        actualScript.Should().Contain(expected:"isAuthenticated: function");
        actualScript.Should().Contain(expected:"document.body.classList.toggle(\"is-authenticated\"");
    }

    [Fact]
    public async Task GetToolsScripts_ReturnsAuthenticatedGridStartupLogic()
    {
        // Given

        // When
        string actualScript = await Client.GetStringAsync(requestUri:"/tools/grids.js");

        // Then
        actualScript.Should().Contain(expected:"WorkflowApi.isAuthenticated()");
        actualScript.Should().Contain(expected:"workflow-auth-changed");
    }

    [Fact]
    public async Task GetToolsStyles_ReturnsLoginGateStyles()
    {
        // Given

        // When
        string actualStyles = await Client.GetStringAsync(requestUri:"/tools/styles.css");

        // Then
        actualStyles.Should().Contain(expected:"body.wf-shell:not(.is-authenticated) .wf-workbench");
        actualStyles.Should().Contain(expected:"body.wf-shell.is-authenticated .wf-login-gate");
        actualStyles.Should().Contain(expected:".wf-logo");
        actualStyles.Should().Contain(expected:"grid-template-rows: auto minmax(0, 1fr)");
        actualStyles.Should().Contain(expected:".wf-nav-item.active");
    }
}