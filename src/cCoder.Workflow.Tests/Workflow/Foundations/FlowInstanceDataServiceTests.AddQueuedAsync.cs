using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Foundations;

public partial class FlowInstanceDataServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWithoutAuthorizationWhenAddQueuedAsync()
    {
        FlowInstanceData flowInstanceData = CreateRandomFlowInstanceData();

        FlowInstanceData submitted = null;

        flowInstanceDataBrokerMock
            .Setup(x =>
                x.AddFlowInstanceDataAsync(
                    It.Is<FlowInstanceData>(candidate =>
                        !ReferenceEquals(candidate, flowInstanceData)
                        && candidate.Id == flowInstanceData.Id
                        && candidate.FlowDefinitionId == flowInstanceData.FlowDefinitionId
                        && candidate.Name == flowInstanceData.Name
                        && candidate.ContextString == flowInstanceData.ContextString
                        && candidate.State == flowInstanceData.State
                        && candidate.ReportingComponentName == flowInstanceData.ReportingComponentName
                        && candidate.Caller == flowInstanceData.Caller
                        && candidate.Start == flowInstanceData.Start
                        && candidate.End == flowInstanceData.End
                        && candidate.FlowDefinition == null
                    )
                )
            )
            .Callback<FlowInstanceData>(candidate => submitted = candidate)
            .ReturnsAsync((FlowInstanceData value) => value);

        FlowInstanceData result = await flowInstanceDataService.AddQueuedAsync(flowInstanceData);

        result.Should().BeSameAs(flowInstanceData);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(flowInstanceData);
        flowInstanceDataBrokerMock.Verify(
            x => x.AddFlowInstanceDataAsync(It.IsAny<FlowInstanceData>()),
            Times.Once);
        flowInstanceDataBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}
