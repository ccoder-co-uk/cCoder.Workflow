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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: new User { Id = "test-user" });

        FlowInstanceData flowInstanceData = CreateRandomFlowInstanceData();

        FlowInstanceData submitted = null;

        flowInstanceDataBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<FlowInstanceData>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "FlowInstanceData_create"));

        flowInstanceDataBrokerMock
            .Setup(expression: x =>
                x.AddFlowInstanceDataAsync(
entity: It.Is<FlowInstanceData>(candidate =>
                        !ReferenceEquals(candidate, flowInstanceData)
                    )
                )
            )
            .Callback<FlowInstanceData>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (FlowInstanceData value) => value);

        // When
        FlowInstanceData result = await flowInstanceDataService.AddAsync(flowInstanceData: flowInstanceData);

        // Then
        result.Should()
            .BeSameAs(expected: flowInstanceData);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: flowInstanceData);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted
            .Should()
            .BeEquivalentTo(
expectation: flowInstanceData,
config: options => options.Excluding(expression: candidate => candidate.Id)
            );

        result
            .Should()
            .BeEquivalentTo(
expectation: flowInstanceData,
config: options => options.Excluding(expression: candidate => candidate.Id)
            );

        flowInstanceDataBrokerMock.Verify(
expression: x =>
                x.AddFlowInstanceDataAsync(
entity: It.Is<FlowInstanceData>(candidate =>
                        !ReferenceEquals(candidate, flowInstanceData)
                    )
                ),
times: Times.Once
        );

        flowInstanceDataBrokerMock.Verify(
expression: x => x.GetAppId(entity: It.IsAny<FlowInstanceData>()),
times: Times.AtMostOnce()
        );

        flowInstanceDataBrokerMock.VerifyNoOtherCalls();

        authorizationBrokerMock.Verify(
expression: x => x.Authorize(appId: (int?)7, privilege: "FlowInstanceData_create"),
times: Times.Once
        );

        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        FlowInstanceData flowInstanceData = CreateRandomFlowInstanceData();

        flowInstanceDataBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<FlowInstanceData>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "FlowInstanceData_create"))
            .Throws(exception: new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await flowInstanceDataService.AddAsync(flowInstanceData: flowInstanceData);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        flowInstanceDataBrokerMock.Verify(
expression: x => x.GetAppId(entity: It.IsAny<FlowInstanceData>()),
times: Times.AtMostOnce()
        );

        flowInstanceDataBrokerMock.VerifyNoOtherCalls();

        authorizationBrokerMock.Verify(
expression: x => x.Authorize(appId: (int?)7, privilege: "FlowInstanceData_create"),
times: Times.Once
        );

        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}