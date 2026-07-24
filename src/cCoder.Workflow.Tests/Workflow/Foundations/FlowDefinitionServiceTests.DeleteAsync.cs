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

public partial class FlowDefinitionServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser()).Returns(value: new User { Id = "test-user" });
        Guid flowDefinitionId = Guid.NewGuid();
        FlowDefinition flowDefinition = CreateRandomFlowDefinition(id: flowDefinitionId, appId: 7);

        flowDefinitionBrokerMock
            .Setup(expression: x => x.GetAllFlowDefinitions(ignoreFilters: true))
            .Returns(value: new[] { flowDefinition }.AsQueryable());

        flowDefinitionBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<FlowDefinition>())).Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "FlowDefinition_delete"));

        flowDefinitionBrokerMock
            .Setup(
expression: x =>
                    x.DeleteFlowDefinitionAsync(
entity: It.Is<FlowDefinition>(candidate => candidate.Id == flowDefinition.Id)
                    )
            )
            .ReturnsAsync(value: 1);

        // When
        await flowDefinitionService.DeleteAsync(id: flowDefinitionId);

        // Then
        flowDefinitionBrokerMock.Verify(expression: x => x.GetAllFlowDefinitions(ignoreFilters: true), times: Times.Once);
        flowDefinitionBrokerMock.Verify(
expression: x =>
                x.DeleteFlowDefinitionAsync(
entity: It.Is<FlowDefinition>(candidate => candidate.Id == flowDefinition.Id)
                ),
times: Times.Once
        );
        flowDefinitionBrokerMock.Verify(
expression: x => x.GetAppId(entity: It.IsAny<FlowDefinition>()),
times: Times.AtMostOnce()
        );
        flowDefinitionBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
expression: x => x.Authorize(appId: (int?)7, privilege: "FlowDefinition_delete"),
times: Times.Once
        );
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Guid flowDefinitionId = Guid.NewGuid();
        FlowDefinition flowDefinition = CreateRandomFlowDefinition(id: flowDefinitionId, appId: 7);

        flowDefinitionBrokerMock
            .Setup(expression: x => x.GetAllFlowDefinitions(ignoreFilters: true))
            .Returns(value: new[] { flowDefinition }.AsQueryable());

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "FlowDefinition_delete"))
            .Throws(exception: new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await flowDefinitionService.DeleteAsync(id: flowDefinitionId);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage(expectedWildcardPattern: "Access Denied!");
        flowDefinitionBrokerMock.Verify(expression: x => x.GetAllFlowDefinitions(ignoreFilters: true), times: Times.Once);
        flowDefinitionBrokerMock.Verify(
expression: x => x.GetAppId(entity: It.IsAny<FlowDefinition>()),
times: Times.AtMostOnce()
        );
        flowDefinitionBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
expression: x => x.Authorize(appId: (int?)7, privilege: "FlowDefinition_delete"),
times: Times.Once
        );
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteWithInstancesAsync()
    {
        Guid flowDefinitionId = Guid.NewGuid();
        FlowDefinition flowDefinition = CreateRandomFlowDefinition(id: flowDefinitionId, appId: 7);

        flowDefinitionBrokerMock
            .Setup(expression: x => x.GetAllFlowDefinitions(ignoreFilters: true))
            .Returns(value: new[] { flowDefinition }.AsQueryable());

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "FlowDefinition_delete"));

        flowDefinitionBrokerMock
            .Setup(expression: x => x.DeleteFlowDefinitionWithInstancesAsync(id: flowDefinitionId))
            .Returns(value: ValueTask.CompletedTask);

        await flowDefinitionService.DeleteWithInstancesAsync(id: flowDefinitionId);

        flowDefinitionBrokerMock.Verify(expression: x => x.GetAllFlowDefinitions(ignoreFilters: true), times: Times.Once);
        flowDefinitionBrokerMock.Verify(
expression: x => x.DeleteFlowDefinitionWithInstancesAsync(id: flowDefinitionId),
times: Times.Once
        );
        flowDefinitionBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
expression: x => x.Authorize(appId: (int?)7, privilege: "FlowDefinition_delete"),
times: Times.Once
        );
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}