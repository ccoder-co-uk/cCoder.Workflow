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
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: new User { Id = "test-user" });

        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        WorkflowEvent submitted = null;

        workflowEventBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<WorkflowEvent>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "WorkflowEvent_create"));

        workflowEventBrokerMock
            .Setup(expression: x =>
                x.AddWorkflowEventAsync(
entity: It.Is<WorkflowEvent>(candidate => !ReferenceEquals(candidate, workflowEvent))
                )
            )
            .Callback<WorkflowEvent>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (WorkflowEvent value) => value);

        // When
        WorkflowEvent result = await workflowEventService.AddAsync(workflowEvent: workflowEvent);

        // Then
        result.Should()
            .BeSameAs(expected: workflowEvent);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: workflowEvent);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted
            .Should()
            .BeEquivalentTo(
expectation: workflowEvent,
config: options =>
                    options
                        .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("CreatedOn")
                        )
                        .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("CreatedBy")
                        )
                        .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdated")
                        )
                        .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdatedBy")
                        )
                        .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdatedOn")
                        )
                        .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("UpdatedBy")
                        )
                        .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("Created")
                        )
                        .Excluding(expression: candidate => candidate.Id)
            );

        result
            .Should()
            .BeEquivalentTo(
expectation: workflowEvent,
config: options =>
                    options
                        .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("CreatedOn")
                        )
                        .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("CreatedBy")
                        )
                        .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdated")
                        )
                        .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdatedBy")
                        )
                        .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdatedOn")
                        )
                        .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("UpdatedBy")
                        )
                        .Excluding(
predicate: (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("Created")
                        )
                        .Excluding(expression: candidate => candidate.Id)
            );

        workflowEventBrokerMock.Verify(
expression: x =>
                x.AddWorkflowEventAsync(
entity: It.Is<WorkflowEvent>(candidate => !ReferenceEquals(candidate, workflowEvent))
                ),
times: Times.Once
        );

        workflowEventBrokerMock.Verify(
expression: x => x.GetAppId(entity: It.IsAny<WorkflowEvent>()),
times: Times.AtMostOnce()
        );

        workflowEventBrokerMock.VerifyNoOtherCalls();

        authorizationBrokerMock.Verify(
expression: x => x.Authorize(appId: (int?)7, privilege: "WorkflowEvent_create"),
times: Times.Once
        );
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        WorkflowEvent workflowEvent = CreateRandomWorkflowEvent();

        workflowEventBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<WorkflowEvent>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "WorkflowEvent_create"))
            .Throws(exception: new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await workflowEventService.AddAsync(workflowEvent: workflowEvent);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        workflowEventBrokerMock.Verify(
expression: x => x.GetAppId(entity: It.IsAny<WorkflowEvent>()),
times: Times.AtMostOnce()
        );

        workflowEventBrokerMock.VerifyNoOtherCalls();

        authorizationBrokerMock.Verify(
expression: x => x.Authorize(appId: (int?)7, privilege: "WorkflowEvent_create"),
times: Times.Once
        );
    }

}