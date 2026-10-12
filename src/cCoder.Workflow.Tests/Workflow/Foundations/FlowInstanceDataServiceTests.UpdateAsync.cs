// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

using System.Security;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Models;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class FlowInstanceDataServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: new User { Id = "test-user" });

        FlowInstanceData flowInstanceData = CreateRandomFlowInstanceData();

        FlowInstanceData submitted = null;

        flowInstanceDataBrokerMock.Setup(expression: x => x.SelectAppId(flowInstanceData: It.IsAny<FlowInstanceData>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.IsAuthorized(appId: (int?)7, privilege: "FlowInstanceData_update"))
            .Returns(value: true);

        flowInstanceDataBrokerMock
            .Setup(expression: x => x.SelectFlowInstanceDataByIdAsync(instanceId: flowInstanceData.Id))
            .ReturnsAsync(value: flowInstanceData);

        flowInstanceDataBrokerMock
            .Setup(expression: x => x.CompareExchangeFlowInstanceDataAsync(workflowInstanceExchange: It.IsAny<WorkflowInstanceExchange>()))
            .Callback<WorkflowInstanceExchange>(action: exchange => submitted = exchange.Replacement)
            .ReturnsAsync(value: 1);

        // When
        FlowInstanceData result = await flowInstanceDataService.UpdateFlowInstanceDataAsync(updatedFlowInstanceData: flowInstanceData);

        // Then
        result.Should()
            .BeSameAs(expected: flowInstanceData);


        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: flowInstanceData);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted.Should()
            .BeEquivalentTo(expectation: flowInstanceData);


        result.Should()
            .BeEquivalentTo(expectation: flowInstanceData);


        flowInstanceDataBrokerMock.Verify(
expression: x => x.CompareExchangeFlowInstanceDataAsync(workflowInstanceExchange: It.IsAny<WorkflowInstanceExchange>()),
times: Times.Once
        );

        flowInstanceDataBrokerMock.Verify(
            expression: x => x.SelectFlowInstanceDataByIdAsync(instanceId: flowInstanceData.Id),
            times: Times.Once);

        flowInstanceDataBrokerMock.Verify(
expression: x => x.SelectAppId(flowInstanceData: It.IsAny<FlowInstanceData>()),
times: Times.AtMostOnce()
        );

        flowInstanceDataBrokerMock.VerifyNoOtherCalls();

        authorizationBrokerMock.Verify(
expression: x => x.IsAuthorized(appId: (int?)7, privilege: "FlowInstanceData_update"),
times: Times.Once
        );

        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        FlowInstanceData flowInstanceData = CreateRandomFlowInstanceData();

        flowInstanceDataBrokerMock.Setup(expression: x => x.SelectAppId(flowInstanceData: It.IsAny<FlowInstanceData>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock
            .Setup(expression: x => x.IsAuthorized(appId: (int?)7, privilege: "FlowInstanceData_update"))
            .Returns(value: false);

        // When
        Func<Task> action = async () => await flowInstanceDataService.UpdateFlowInstanceDataAsync(updatedFlowInstanceData: flowInstanceData);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        flowInstanceDataBrokerMock.Verify(
expression: x => x.SelectAppId(flowInstanceData: It.IsAny<FlowInstanceData>()),
times: Times.AtMostOnce()
        );

        flowInstanceDataBrokerMock.VerifyNoOtherCalls();

        authorizationBrokerMock.Verify(
expression: x => x.IsAuthorized(appId: (int?)7, privilege: "FlowInstanceData_update"),
times: Times.Once
        );

        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}