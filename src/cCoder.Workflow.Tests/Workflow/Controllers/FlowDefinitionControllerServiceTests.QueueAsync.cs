// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests.Workflow.Controllers;

public partial class FlowDefinitionControllerServiceTests
{
    [Fact]
    public async Task ShouldQueueWithCurrentUserIdWhenCallerIsMissing()
    {
        Guid flowId = Guid.NewGuid();
        Guid queuedId = Guid.NewGuid();
        User currentUser = new() { Id = "admin" };

        authorizationBrokerMock
            .Setup(expression:broker => broker.GetCurrentUser())
            .Returns(value:currentUser);
        flowDefinitionCoordinationServiceMock
            .Setup(expression:service => service.QueueAsync(flowId, currentUser.Id, "{}"))
            .ReturnsAsync(value:queuedId);

        Guid result = await service.QueueAsync(id:flowId, asUserId:null, args:"{}");

        result.Should().Be(expected:queuedId);
        authorizationBrokerMock.Verify(expression:broker => broker.GetCurrentUser(), times:Times.Once);
        flowDefinitionCoordinationServiceMock.Verify(
expression:            foundService => foundService.QueueAsync(flowId, currentUser.Id, "{}"),
times:            Times.Once);
        flowDefinitionCoordinationServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldQueueWithProvidedCallerIdWhenCallerIsPresent()
    {
        Guid flowId = Guid.NewGuid();
        Guid queuedId = Guid.NewGuid();

        flowDefinitionCoordinationServiceMock
            .Setup(expression:service => service.QueueAsync(flowId, "ash", "{}"))
            .ReturnsAsync(value:queuedId);

        Guid result = await service.QueueAsync(id:flowId, asUserId:"ash", args:"{}");

        result.Should().Be(expected:queuedId);
        flowDefinitionCoordinationServiceMock.Verify(
expression:            foundService => foundService.QueueAsync(flowId, "ash", "{}"),
times:            Times.Once);
        flowDefinitionCoordinationServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}