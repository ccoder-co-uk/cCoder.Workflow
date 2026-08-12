// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

#pragma warning disable STXFORMAT009
public partial class FlowDefinitionProcessingServiceTests
{
    [Fact]
    public async Task ShouldAddAndUpdateFlowDefinitionsAsync()
    {
        // Given
        FlowDefinition added = CreateRandomFlowDefinition();
        added.Id = Guid.Empty;
        FlowDefinition updated = CreateRandomFlowDefinition();

        jsonBrokerMock
            .Setup(expression: broker => broker.Serialize(
                value: It.IsAny<object>()))
            .Returns(value: "[]");

        loggingBrokerMock
            .Setup(expression: broker => broker.LogDebug(
                message: "AddOrUpdate:\n[]",
                args: It.IsAny<object[]>()));

        flowDefinitionServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[] { updated }.AsQueryable());

        flowDefinitionServiceMock
            .Setup(expression: service => service.AddFlowDefinitionAsync(
                newFlowDefinition: added))
            .Returns(value: ValueTask.FromResult(result: added));

        flowDefinitionServiceMock
            .Setup(expression: service => service.UpdateFlowDefinitionAsync(
                updatedFlowDefinition: updated))
            .Returns(value: ValueTask.FromResult(result: updated));

        // When
        Result<FlowDefinition>[] results = (await flowDefinitionProcessingService
            .AddOrUpdateFlowDefinition(items: new[] { added, updated }))
            .ToArray();

        // Then
        results.Should().OnlyContain(predicate: result => result.Success);
        results[0].Message.Should().Be(expected: "Added Successfully");
        results[1].Message.Should().Be(expected: "Updated Successfully");
        flowDefinitionServiceMock.VerifyAll();
        jsonBrokerMock.VerifyAll();
        loggingBrokerMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldCaptureAddOrUpdateFlowDefinitionFailureAsync()
    {
        // Given
        FlowDefinition item = CreateRandomFlowDefinition();
        item.Id = Guid.Empty;

        jsonBrokerMock
            .Setup(expression: broker => broker.Serialize(
                value: It.IsAny<object>()))
            .Returns(value: "[]");

        flowDefinitionServiceMock
            .Setup(expression: service => service.AddFlowDefinitionAsync(
                newFlowDefinition: item))
            .Throws(exception: new Exception(message: "failed"));

        // When
        Result<FlowDefinition> result = (await flowDefinitionProcessingService
            .AddOrUpdateFlowDefinition(items: new[] { item }))
            .Single();

        // Then
        result.Success.Should().BeFalse();
        result.Item.Should().BeSameAs(expected: item);
        result.Message.Should().Be(expected: "The Workflow service failed.");
    }

    [Fact]
    public async Task ShouldDeleteFlowDefinitionsByAppIdAsync()
    {
        // Given
        flowDefinitionServiceMock
            .Setup(expression: service => service
                .DeleteWithInstancesByAppIdAsync(appId: 7))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await flowDefinitionProcessingService.DeleteByAppIdAsync(appId: 7);

        // Then
        flowDefinitionServiceMock.VerifyAll();
    }
}
#pragma warning restore STXFORMAT009