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
            .Setup(broker => broker.GetCurrentUser())
            .Returns(currentUser);
        flowDefinitionCoordinationServiceMock
            .Setup(service => service.QueueAsync(flowId, currentUser.Id, "{}"))
            .ReturnsAsync(queuedId);

        Guid result = await service.QueueAsync(flowId, null, "{}");

        result.Should().Be(queuedId);
        authorizationBrokerMock.Verify(broker => broker.GetCurrentUser(), Times.Once);
        flowDefinitionCoordinationServiceMock.Verify(
            foundService => foundService.QueueAsync(flowId, currentUser.Id, "{}"),
            Times.Once);
        flowDefinitionCoordinationServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldQueueWithProvidedCallerIdWhenCallerIsPresent()
    {
        Guid flowId = Guid.NewGuid();
        Guid queuedId = Guid.NewGuid();

        flowDefinitionCoordinationServiceMock
            .Setup(service => service.QueueAsync(flowId, "ash", "{}"))
            .ReturnsAsync(queuedId);

        Guid result = await service.QueueAsync(flowId, "ash", "{}");

        result.Should().Be(queuedId);
        flowDefinitionCoordinationServiceMock.Verify(
            foundService => foundService.QueueAsync(flowId, "ash", "{}"),
            Times.Once);
        flowDefinitionCoordinationServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}
