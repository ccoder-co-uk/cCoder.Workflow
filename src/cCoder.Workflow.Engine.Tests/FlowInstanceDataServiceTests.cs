// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading.Tasks;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Models.Exceptions;
using cCoder.Workflow.Engine.Services.Foundations;
using Moq;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class FlowInstanceDataServiceTests
{
    private const string ApiRoot = "https://localhost/Api/";
    private const string Token = "test-token";

    private readonly Mock<IWorkflowHttpClientBroker> broker = new(MockBehavior.Strict);

    private FlowInstanceDataService CreateService() =>
        new(workflowHttpClientBroker: broker.Object, jsonBroker: new JsonBroker());

    [Fact]
    public async Task ShouldRejectMissingWorkflowIdentifiersWithoutCallingTheApi()
    {
        // Given
        FlowInstanceDataService service = CreateService();

        // When
        Func<Task> getInstance = () => service.GetFlowInstanceDataAsync(
            apiRoot: ApiRoot, authToken: Token, flowInstanceDataId: Guid.Empty);

        Func<Task> getApp = () => service.GetPortalAppJsonAsync(
            apiRoot: ApiRoot, authToken: Token, appId: 0);

        Func<Task> getCaller = () => service.GetCallerJsonAsync(apiRoot: ApiRoot, authToken: " ");

        Func<Task> save = async () => await service.SaveFlowInstanceDataAsync(
            flowInstanceData: null, apiRoot: ApiRoot, authToken: Token);

        // Then
        await Assert.ThrowsAsync<WorkflowEngineValidationException>(testCode: getInstance);
        await Assert.ThrowsAsync<WorkflowEngineValidationException>(testCode: getApp);
        await Assert.ThrowsAsync<WorkflowEngineValidationException>(testCode: getCaller);
        await Assert.ThrowsAsync<WorkflowEngineValidationException>(testCode: save);
        broker.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldRejectEmptyInstanceResponse()
    {
        // Given
        Guid instanceId = Guid.NewGuid();

        broker.Setup(expression: client => client.GetStringAsync(
                apiRoot: ApiRoot, authToken: Token, requestUri: $"Workflow/FlowInstanceData({instanceId})?$expand=FlowDefinition($expand=App)"))
            .Returns(value: ValueTask.FromResult(result: "null"));

        FlowInstanceDataService service = CreateService();

        // When
        Func<Task> get = () => service.GetFlowInstanceDataAsync(
            apiRoot: ApiRoot, authToken: Token, flowInstanceDataId: instanceId);

        // Then
        WorkflowEngineDependencyException error = await Assert.ThrowsAsync<WorkflowEngineDependencyException>(testCode: get);
        Assert.IsType<InvalidOperationException>(@object: error.InnerException);
        broker.VerifyAll();
    }

    [Fact]
    public async Task ShouldReportRejectedStateUpdateAsServiceFailure()
    {
        // Given
        FlowInstanceData instance = new() { Id = Guid.NewGuid() };
        WorkflowHttpResult rejection = new() { IsSuccess = false, StatusCode = 403, Status = "Forbidden", Body = "Denied" };

        broker.Setup(expression: client => client.PutJsonAsync(
                apiRoot: ApiRoot, authToken: Token, requestUri: $"Workflow/FlowInstanceData({instance.Id})",
                payload: It.IsAny<string>()))
            .Returns(value: ValueTask.FromResult(result: rejection));

        FlowInstanceDataService service = CreateService();

        // When
        Func<Task> save = async () => await service.SaveFlowInstanceDataAsync(
            flowInstanceData: instance, apiRoot: ApiRoot, authToken: Token);

        // Then
        WorkflowEngineServiceException error = await Assert.ThrowsAsync<WorkflowEngineServiceException>(testCode: save);
        Assert.Contains(expectedSubstring: "403", actualString: error.InnerException.Message);
        broker.VerifyAll();
    }

    [Fact]
    public async Task ShouldPreserveCallerTransportFailure()
    {
        // Given
        HttpRequestException transportFailure = new("Transport failed");

        broker.Setup(expression: client => client.GetStringAsync(
                apiRoot: ApiRoot, authToken: Token, requestUri: "AppSecurity/User/Me()"))
            .Throws(exception: transportFailure);

        FlowInstanceDataService service = CreateService();

        // When
        Func<Task> get = () => service.GetCallerJsonAsync(apiRoot: ApiRoot, authToken: Token);

        // Then
        WorkflowEngineServiceException error = await Assert.ThrowsAsync<WorkflowEngineServiceException>(testCode: get);
        Assert.Same(expected: transportFailure, actual: error.InnerException);
        broker.VerifyAll();
    }
}