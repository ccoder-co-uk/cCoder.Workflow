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
        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(value: new User { Id = "test-user" });

        FlowDefinition flowDefinition = CreateRandomFlowDefinition(appId: 7);

        FlowDefinition submitted = null;

        flowDefinitionBrokerMock.Setup(expression: x => x.SelectAppId(entity: It.IsAny<FlowDefinition>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "FlowDefinition_create"));

        flowDefinitionBrokerMock
            .Setup(expression: x =>
                x.AddFlowDefinitionAsync(
newEntity: It.Is<FlowDefinition>(match: candidate => !ReferenceEquals(objA: candidate, objB: flowDefinition))
                )
            )
            .Callback<FlowDefinition>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (FlowDefinition value) => value);

        // When
        FlowDefinition result = await flowDefinitionService.AddFlowDefinitionAsync(newFlowDefinition: flowDefinition);

        // Then
        result.Should()
            .BeSameAs(expected: flowDefinition);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: flowDefinition);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        var expectedValues = new
        {
            flowDefinition.Name,
            flowDefinition.Description,
            flowDefinition.AppId,
            flowDefinition.DefinitionJson,
            flowDefinition.ConfigJson,
            flowDefinition.ReportingComponentName,
            flowDefinition.InstanceReportingComponentName
        };

        var submittedValues = new
        {
            submitted.Name,
            submitted.Description,
            submitted.AppId,
            submitted.DefinitionJson,
            submitted.ConfigJson,
            submitted.ReportingComponentName,
            submitted.InstanceReportingComponentName
        };

        submittedValues.Should()
            .BeEquivalentTo(expectation: expectedValues);

        var resultValues = new
        {
            result.Name,
            result.Description,
            result.AppId,
            result.DefinitionJson,
            result.ConfigJson,
            result.ReportingComponentName,
            result.InstanceReportingComponentName
        };

        resultValues.Should()
            .BeEquivalentTo(expectation: expectedValues);

        flowDefinitionBrokerMock.Verify(
expression: x =>
                x.AddFlowDefinitionAsync(
newEntity: It.Is<FlowDefinition>(match: candidate => !ReferenceEquals(objA: candidate, objB: flowDefinition))
                ),
times: Times.Once
        );

        flowDefinitionBrokerMock.Verify(
expression: x => x.SelectAppId(entity: It.IsAny<FlowDefinition>()),
times: Times.AtMostOnce()
        );

        flowDefinitionBrokerMock.VerifyNoOtherCalls();

        authorizationBrokerMock.Verify(
expression: x => x.Authorize(appId: (int?)7, privilege: "FlowDefinition_create"),
times: Times.Once
        );
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        FlowDefinition flowDefinition = CreateRandomFlowDefinition(appId: 7);

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "FlowDefinition_create"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await flowDefinitionService.AddFlowDefinitionAsync(newFlowDefinition: flowDefinition);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        flowDefinitionBrokerMock.Verify(
expression: x => x.SelectAppId(entity: It.IsAny<FlowDefinition>()),
times: Times.AtMostOnce()
        );

        flowDefinitionBrokerMock.VerifyNoOtherCalls();

        authorizationBrokerMock.Verify(
expression: x => x.Authorize(appId: (int?)7, privilege: "FlowDefinition_create"),
times: Times.Once
        );
    }

}