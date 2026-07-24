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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        authorizationBrokerMock.Setup(expression:x => x.GetCurrentUser()).Returns(value:new User { Id = "test-user" });
        FlowDefinition flowDefinition = CreateRandomFlowDefinition(appId: 7);

        FlowDefinition submitted = null;

        flowDefinitionBrokerMock.Setup(expression:x => x.GetAppId(It.IsAny<FlowDefinition>())).Returns(value:(int?)7);

        authorizationBrokerMock.Setup(expression:x => x.Authorize((int?)7, "FlowDefinition_update"));

        flowDefinitionBrokerMock
            .Setup(expression:x => x.UpdateFlowDefinitionAsync(It.IsAny<FlowDefinition>()))
            .Callback<FlowDefinition>(action:candidate => submitted = candidate)
            .ReturnsAsync(valueFunction:(FlowDefinition value) => value);

        // When
        FlowDefinition result = await flowDefinitionService.UpdateAsync(flowDefinition:flowDefinition);

        // Then
        result.Should().BeSameAs(expected:flowDefinition);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(unexpected:flowDefinition);
        result.Should().NotBeSameAs(unexpected:submitted);

        submitted
            .Should()
            .BeEquivalentTo(
expectation:                flowDefinition,
config:                options =>
                    options
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("CreatedOn")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("CreatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdated")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdatedOn")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("UpdatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("Created")
                        )
            );

        result
            .Should()
            .BeEquivalentTo(
expectation:                flowDefinition,
config:                options =>
                    options
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("CreatedOn")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("CreatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdated")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdatedOn")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("UpdatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("Created")
                        )
            );

        flowDefinitionBrokerMock.Verify(
expression:            x => x.UpdateFlowDefinitionAsync(It.IsAny<FlowDefinition>()),
times:            Times.Once
        );
        flowDefinitionBrokerMock.Verify(
expression:            x => x.GetAppId(It.IsAny<FlowDefinition>()),
times:            Times.AtMostOnce()
        );
        flowDefinitionBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
expression:            x => x.Authorize((int?)7, "FlowDefinition_update"),
times:            Times.Once
        );
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        FlowDefinition flowDefinition = CreateRandomFlowDefinition(appId: 7);

        authorizationBrokerMock
            .Setup(expression:x => x.Authorize((int?)7, "FlowDefinition_update"))
            .Throws(exception:new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await flowDefinitionService.UpdateAsync(flowDefinition:flowDefinition);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage(expectedWildcardPattern:"Access Denied!");
        flowDefinitionBrokerMock.Verify(
expression:            x => x.GetAppId(It.IsAny<FlowDefinition>()),
times:            Times.AtMostOnce()
        );
        flowDefinitionBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
expression:            x => x.Authorize((int?)7, "FlowDefinition_update"),
times:            Times.Once
        );
    }

}