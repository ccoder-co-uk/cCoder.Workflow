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
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(new User { Id = "test-user" });
        Guid flowInstanceDataId = Guid.NewGuid();
        FlowInstanceData flowInstanceData = CreateRandomFlowInstanceData(id: flowInstanceDataId);

        flowInstanceDataBrokerMock
            .Setup(x => x.GetAllFlowInstanceData(false))
            .Returns(new[] { flowInstanceData }.AsQueryable());

        flowInstanceDataBrokerMock.Setup(x => x.GetAppId(It.IsAny<FlowInstanceData>())).Returns((int?)7);

        flowInstanceDataBrokerMock.Setup(x => x.GetAppId(It.IsAny<FlowInstanceData>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "FlowInstanceData_delete"));

        flowInstanceDataBrokerMock
            .Setup(
                x =>
                    x.DeleteFlowInstanceDataAsync(
                        It.Is<FlowInstanceData>(candidate => candidate.Id == flowInstanceData.Id)
                    )
            )
            .ReturnsAsync(1);

        // When
        await flowInstanceDataService.DeleteAsync(flowInstanceDataId);

        // Then
        flowInstanceDataBrokerMock.Verify(x => x.GetAllFlowInstanceData(false), Times.Once);
        flowInstanceDataBrokerMock.Verify(
            x =>
                x.DeleteFlowInstanceDataAsync(
                    It.Is<FlowInstanceData>(candidate => candidate.Id == flowInstanceData.Id)
                ),
            Times.Once
        );
        flowInstanceDataBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<FlowInstanceData>()),
            Times.AtMostOnce()
        );
        flowInstanceDataBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
            x => x.Authorize((int?)7, "FlowInstanceData_delete"),
            Times.Once
        );
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Guid flowInstanceDataId = Guid.NewGuid();
        FlowInstanceData flowInstanceData = CreateRandomFlowInstanceData(id: flowInstanceDataId);

        flowInstanceDataBrokerMock
            .Setup(x => x.GetAllFlowInstanceData(false))
            .Returns(new[] { flowInstanceData }.AsQueryable());

        flowInstanceDataBrokerMock.Setup(x => x.GetAppId(It.IsAny<FlowInstanceData>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "FlowInstanceData_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () =>
            await flowInstanceDataService.DeleteAsync(flowInstanceDataId);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        flowInstanceDataBrokerMock.Verify(x => x.GetAllFlowInstanceData(false), Times.Once);
        flowInstanceDataBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<FlowInstanceData>()),
            Times.AtMostOnce()
        );
        flowInstanceDataBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
            x => x.Authorize((int?)7, "FlowInstanceData_delete"),
            Times.Once
        );
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}










