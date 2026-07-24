// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        // Given
        FlowInstanceData flowInstanceData = CreateRandomFlowInstanceData();

        FlowInstanceData submitted = null;

        flowInstanceDataBrokerMock
            .Setup(expression: x =>
                x.AddFlowInstanceDataAsync(
newEntity: It.Is<FlowInstanceData>(match: candidate =>
                        !ReferenceEquals(objA: candidate, objB: flowInstanceData)
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
            .Callback<FlowInstanceData>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (FlowInstanceData value) => value);

        // When
        FlowInstanceData result = await flowInstanceDataService.AddQueuedFlowInstanceDataAsync(newFlowInstanceData: flowInstanceData);

        // Then
        result.Should()
            .BeSameAs(expected: flowInstanceData);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: flowInstanceData);

        flowInstanceDataBrokerMock.Verify(
expression: x => x.AddFlowInstanceDataAsync(newEntity: It.IsAny<FlowInstanceData>()),
times: Times.Once);

        flowInstanceDataBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }
}