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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(new User { Id = "test-user" });
        FlowDefinition flowDefinition = CreateRandomFlowDefinition(appId: 7);

        FlowDefinition submitted = null;

        flowDefinitionBrokerMock.Setup(x => x.GetAppId(It.IsAny<FlowDefinition>())).Returns((int?)7);

        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "FlowDefinition_create"));

        flowDefinitionBrokerMock
            .Setup(x =>
                x.AddFlowDefinitionAsync(
                    It.Is<FlowDefinition>(candidate => !ReferenceEquals(candidate, flowDefinition))
                )
            )
            .Callback<FlowDefinition>(candidate => submitted = candidate)
            .ReturnsAsync((FlowDefinition value) => value);

        // When
        FlowDefinition result = await flowDefinitionService.AddAsync(flowDefinition);

        // Then
        result.Should().BeSameAs(flowDefinition);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(flowDefinition);
        result.Should().NotBeSameAs(submitted);

        submitted
            .Should()
            .BeEquivalentTo(
                flowDefinition,
                options =>
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
                        .Excluding(candidate => candidate.Id)
            );

        result
            .Should()
            .BeEquivalentTo(
                flowDefinition,
                options =>
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
                        .Excluding(candidate => candidate.Id)
            );

        flowDefinitionBrokerMock.Verify(
            x =>
                x.AddFlowDefinitionAsync(
                    It.Is<FlowDefinition>(candidate => !ReferenceEquals(candidate, flowDefinition))
                ),
            Times.Once
        );
        flowDefinitionBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<FlowDefinition>()),
            Times.AtMostOnce()
        );
        flowDefinitionBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
            x => x.Authorize((int?)7, "FlowDefinition_create"),
            Times.Once
        );
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        FlowDefinition flowDefinition = CreateRandomFlowDefinition(appId: 7);

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "FlowDefinition_create"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await flowDefinitionService.AddAsync(flowDefinition);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        flowDefinitionBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<FlowDefinition>()),
            Times.AtMostOnce()
        );
        flowDefinitionBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
            x => x.Authorize((int?)7, "FlowDefinition_create"),
            Times.Once
        );
    }

}