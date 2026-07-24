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
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(new User { Id = "test-user" });
        Guid flowDefinitionId = Guid.NewGuid();
        FlowDefinition flowDefinition = CreateRandomFlowDefinition(id: flowDefinitionId, appId: 7);

        flowDefinitionBrokerMock
            .Setup(x => x.GetAllFlowDefinitions(true))
            .Returns(new[] { flowDefinition }.AsQueryable());

        flowDefinitionBrokerMock.Setup(x => x.GetAppId(It.IsAny<FlowDefinition>())).Returns((int?)7);

        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "FlowDefinition_delete"));

        flowDefinitionBrokerMock
            .Setup(
                x =>
                    x.DeleteFlowDefinitionAsync(
                        It.Is<FlowDefinition>(candidate => candidate.Id == flowDefinition.Id)
                    )
            )
            .ReturnsAsync(1);

        // When
        await flowDefinitionService.DeleteAsync(flowDefinitionId);

        // Then
        flowDefinitionBrokerMock.Verify(x => x.GetAllFlowDefinitions(true), Times.Once);
        flowDefinitionBrokerMock.Verify(
            x =>
                x.DeleteFlowDefinitionAsync(
                    It.Is<FlowDefinition>(candidate => candidate.Id == flowDefinition.Id)
                ),
            Times.Once
        );
        flowDefinitionBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<FlowDefinition>()),
            Times.AtMostOnce()
        );
        flowDefinitionBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
            x => x.Authorize((int?)7, "FlowDefinition_delete"),
            Times.Once
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
            .Setup(x => x.GetAllFlowDefinitions(true))
            .Returns(new[] { flowDefinition }.AsQueryable());

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "FlowDefinition_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await flowDefinitionService.DeleteAsync(flowDefinitionId);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        flowDefinitionBrokerMock.Verify(x => x.GetAllFlowDefinitions(true), Times.Once);
        flowDefinitionBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<FlowDefinition>()),
            Times.AtMostOnce()
        );
        flowDefinitionBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
            x => x.Authorize((int?)7, "FlowDefinition_delete"),
            Times.Once
        );
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteWithInstancesAsync()
    {
        Guid flowDefinitionId = Guid.NewGuid();
        FlowDefinition flowDefinition = CreateRandomFlowDefinition(id: flowDefinitionId, appId: 7);

        flowDefinitionBrokerMock
            .Setup(x => x.GetAllFlowDefinitions(true))
            .Returns(new[] { flowDefinition }.AsQueryable());

        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "FlowDefinition_delete"));

        flowDefinitionBrokerMock
            .Setup(x => x.DeleteFlowDefinitionWithInstancesAsync(flowDefinitionId))
            .Returns(ValueTask.CompletedTask);

        await flowDefinitionService.DeleteWithInstancesAsync(flowDefinitionId);

        flowDefinitionBrokerMock.Verify(x => x.GetAllFlowDefinitions(true), Times.Once);
        flowDefinitionBrokerMock.Verify(
            x => x.DeleteFlowDefinitionWithInstancesAsync(flowDefinitionId),
            Times.Once
        );
        flowDefinitionBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
            x => x.Authorize((int?)7, "FlowDefinition_delete"),
            Times.Once
        );
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}