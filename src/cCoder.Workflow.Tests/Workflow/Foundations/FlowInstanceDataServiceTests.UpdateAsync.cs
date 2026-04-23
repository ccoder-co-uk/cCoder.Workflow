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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(new User { Id = "test-user" });
        FlowInstanceData flowInstanceData = CreateRandomFlowInstanceData();

        FlowInstanceData submitted = null;

        flowInstanceDataBrokerMock.Setup(x => x.GetAppId(It.IsAny<FlowInstanceData>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "FlowInstanceData_update"));

        flowInstanceDataBrokerMock
            .Setup(x => x.UpdateFlowInstanceDataAsync(It.IsAny<FlowInstanceData>()))
            .Callback<FlowInstanceData>(candidate => submitted = candidate)
            .ReturnsAsync((FlowInstanceData value) => value);

        // When
        FlowInstanceData result = await flowInstanceDataService.UpdateAsync(flowInstanceData);

        // Then
        result.Should().BeSameAs(flowInstanceData);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(flowInstanceData);
        result.Should().NotBeSameAs(submitted);
        submitted.Should().BeEquivalentTo(flowInstanceData);
        result.Should().BeEquivalentTo(flowInstanceData);
        flowInstanceDataBrokerMock.Verify(
            x => x.UpdateFlowInstanceDataAsync(It.IsAny<FlowInstanceData>()),
            Times.Once
        );
        flowInstanceDataBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<FlowInstanceData>()),
            Times.AtMostOnce()
        );
        flowInstanceDataBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
            x => x.Authorize((int?)7, "FlowInstanceData_update"),
            Times.Once
        );
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        FlowInstanceData flowInstanceData = CreateRandomFlowInstanceData();

        flowInstanceDataBrokerMock.Setup(x => x.GetAppId(It.IsAny<FlowInstanceData>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "FlowInstanceData_update"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await flowInstanceDataService.UpdateAsync(flowInstanceData);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        flowInstanceDataBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<FlowInstanceData>()),
            Times.AtMostOnce()
        );
        flowInstanceDataBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
            x => x.Authorize((int?)7, "FlowInstanceData_update"),
            Times.Once
        );
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}











