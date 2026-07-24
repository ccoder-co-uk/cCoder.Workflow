// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Dependencies.ServiceProviders;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests.Workflow.Aggregations;

public partial class FlowDefinitionAggregationServiceTests
{
    [Fact]
    public async Task ShouldQueueWithCurrentUserIdWhenCallerIsMissing()
    {
        // given
        Guid flowId = Guid.NewGuid();
        Guid queuedId = Guid.NewGuid();
        User currentUser = new() { Id = "admin" };

        serviceProviderBrokerMock
            .Setup(expression: broker => broker.GetOperationService<IAuthorizationBroker>(
                FlowDefinitionOperation.Authorization))
            .Returns(value: authorizationBrokerMock.Object);

        authorizationBrokerMock
            .Setup(expression: broker => broker.GetCurrentUser())
            .Returns(value: currentUser);

        flowDefinitionCoordinationServiceMock
            .Setup(expression: service => service.QueueAsync(
                flowDefinitionId: flowId,
                asUserId: currentUser.Id,
                args: "{}"))
            .ReturnsAsync(value: queuedId);

        // when
        Guid result = await service.QueueFlowDefinitionAsync(
            flowDefinitionId: flowId,
            asUserId: null,
            args: "{}");

        // then
        result.Should()
            .Be(expected: queuedId);

        authorizationBrokerMock.Verify(expression: broker => broker.GetCurrentUser(), times: Times.Once);

        flowDefinitionCoordinationServiceMock.Verify(
            expression: foundService => foundService.QueueAsync(
                flowDefinitionId: flowId,
                asUserId: currentUser.Id,
                args: "{}"),
            times: Times.Once);

        flowDefinitionCoordinationServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
        serviceProviderBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldQueueWithProvidedCallerIdWhenCallerIsPresent()
    {
        // given
        Guid flowId = Guid.NewGuid();
        Guid queuedId = Guid.NewGuid();

        flowDefinitionCoordinationServiceMock
            .Setup(expression: service => service.QueueAsync(
                flowDefinitionId: flowId,
                asUserId: "ash",
                args: "{}"))
            .ReturnsAsync(value: queuedId);

        // when
        Guid result = await service.QueueFlowDefinitionAsync(
            flowDefinitionId: flowId,
            asUserId: "ash",
            args: "{}");

        // then
        result.Should()
            .Be(expected: queuedId);

        flowDefinitionCoordinationServiceMock.Verify(
            expression: foundService => foundService.QueueAsync(
                flowDefinitionId: flowId,
                asUserId: "ash",
                args: "{}"),
            times: Times.Once);

        flowDefinitionCoordinationServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
        serviceProviderBrokerMock.VerifyAll();
    }
}