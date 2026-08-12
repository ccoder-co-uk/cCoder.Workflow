// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Engine.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class FlowResultProcessingServiceTests
{
    [Theory]
    [MemberData(
        nameof(WorkflowRequestOrchestrationServiceTests.ExceptionMappings),
        MemberType = typeof(WorkflowRequestOrchestrationServiceTests))]
    public async Task ShouldMapSaveFlowInstanceDataAsyncFailure(
        Exception exception,
        Type expectedType)
    {
        // Given
        const string apiRoot = "https://localhost/";
        const string authToken = "token";
        FlowInstanceData instanceData = CreateFlowInstanceData();

        workflowHttpClientBrokerMock
            .Setup(expression: broker => broker.PutJsonAsync(
                apiRoot: apiRoot,
                authToken: authToken,
                requestUri: It.IsAny<string>(),
                payload: It.IsAny<string>()))
            .Throws(exception: exception);

        var service = CreateService();

        // When
        Func<Task> action = async () => await service
            .SaveFlowInstanceDataAsync(
                flowInstanceData: instanceData,
                apiRoot: apiRoot,
                authToken: authToken);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }
}