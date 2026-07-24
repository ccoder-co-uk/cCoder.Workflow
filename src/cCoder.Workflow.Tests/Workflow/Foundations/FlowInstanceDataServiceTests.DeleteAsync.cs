// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class FlowInstanceDataServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: new User { Id = "test-user" });

        Guid flowInstanceDataId = Guid.NewGuid();
        FlowInstanceData flowInstanceData = CreateRandomFlowInstanceData(flowInstanceDataId: flowInstanceDataId);

        flowInstanceDataBrokerMock
            .Setup(expression: x => x.GetAllFlowInstanceData(ignoreFilters: false))
            .Returns(value: new[] { flowInstanceData }.AsQueryable());

        flowInstanceDataBrokerMock.Setup(expression: x => x.SelectAppId(entity: It.IsAny<FlowInstanceData>()))
            .Returns(value: (int?)7);

        flowInstanceDataBrokerMock.Setup(expression: x => x.SelectAppId(entity: It.IsAny<FlowInstanceData>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "FlowInstanceData_delete"));

        flowInstanceDataBrokerMock
            .Setup(
expression: x =>
                    x.DeleteFlowInstanceDataAsync(
entity: It.Is<FlowInstanceData>(candidate => candidate.Id == flowInstanceData.Id)
                    )
            )
            .ReturnsAsync(value: 1);

        // When
        await flowInstanceDataService.DeleteAsync(flowInstanceDataId: flowInstanceDataId);

        // Then
        flowInstanceDataBrokerMock.Verify(expression: x => x.GetAllFlowInstanceData(ignoreFilters: false), times: Times.Once);

        flowInstanceDataBrokerMock.Verify(
expression: x =>
                x.DeleteFlowInstanceDataAsync(
entity: It.Is<FlowInstanceData>(candidate => candidate.Id == flowInstanceData.Id)
                ),
times: Times.Once
        );

        flowInstanceDataBrokerMock.Verify(
expression: x => x.SelectAppId(entity: It.IsAny<FlowInstanceData>()),
times: Times.AtMostOnce()
        );

        flowInstanceDataBrokerMock.VerifyNoOtherCalls();

        authorizationBrokerMock.Verify(
expression: x => x.Authorize(appId: (int?)7, privilege: "FlowInstanceData_delete"),
times: Times.Once
        );

        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Guid flowInstanceDataId = Guid.NewGuid();
        FlowInstanceData flowInstanceData = CreateRandomFlowInstanceData(flowInstanceDataId: flowInstanceDataId);

        flowInstanceDataBrokerMock
            .Setup(expression: x => x.GetAllFlowInstanceData(ignoreFilters: false))
            .Returns(value: new[] { flowInstanceData }.AsQueryable());

        flowInstanceDataBrokerMock.Setup(expression: x => x.SelectAppId(entity: It.IsAny<FlowInstanceData>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "FlowInstanceData_delete"))
            .Throws(exception: new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () =>
            await flowInstanceDataService.DeleteAsync(flowInstanceDataId: flowInstanceDataId);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        flowInstanceDataBrokerMock.Verify(expression: x => x.GetAllFlowInstanceData(ignoreFilters: false), times: Times.Once);

        flowInstanceDataBrokerMock.Verify(
expression: x => x.SelectAppId(entity: It.IsAny<FlowInstanceData>()),
times: Times.AtMostOnce()
        );

        flowInstanceDataBrokerMock.VerifyNoOtherCalls();

        authorizationBrokerMock.Verify(
expression: x => x.Authorize(appId: (int?)7, privilege: "FlowInstanceData_delete"),
times: Times.Once
        );

        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}