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

public partial class WorkflowEventServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(new User { Id = "test-user" });
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        WorkflowEvent submitted = null;

        workflowEventBrokerMock.Setup(x => x.GetAppId(It.IsAny<WorkflowEvent>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "WorkflowEvent_create"));

        workflowEventBrokerMock
            .Setup(x =>
                x.AddWorkflowEventAsync(
                    It.Is<WorkflowEvent>(candidate => !ReferenceEquals(candidate, workflowEvent))
                )
            )
            .Callback<WorkflowEvent>(candidate => submitted = candidate)
            .ReturnsAsync((WorkflowEvent value) => value);

        // When
        WorkflowEvent result = await workflowEventService.AddAsync(workflowEvent);

        // Then
        result.Should().BeSameAs(workflowEvent);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(workflowEvent);
        result.Should().NotBeSameAs(submitted);

        submitted
            .Should()
            .BeEquivalentTo(
                workflowEvent,
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
                workflowEvent,
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

        workflowEventBrokerMock.Verify(
            x =>
                x.AddWorkflowEventAsync(
                    It.Is<WorkflowEvent>(candidate => !ReferenceEquals(candidate, workflowEvent))
                ),
            Times.Once
        );
        workflowEventBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<WorkflowEvent>()),
            Times.AtMostOnce()
        );
        workflowEventBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
            x => x.Authorize((int?)7, "WorkflowEvent_create"),
            Times.Once
        );
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventBrokerMock.Setup(x => x.GetAppId(It.IsAny<WorkflowEvent>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "WorkflowEvent_create"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await workflowEventService.AddAsync(workflowEvent);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        workflowEventBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<WorkflowEvent>()),
            Times.AtMostOnce()
        );
        workflowEventBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
            x => x.Authorize((int?)7, "WorkflowEvent_create"),
            Times.Once
        );
    }

}