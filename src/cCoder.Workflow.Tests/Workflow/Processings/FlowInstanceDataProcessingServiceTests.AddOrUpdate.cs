// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Workflow.Processings;

#pragma warning disable STXFORMAT005, STXFORMAT009
public partial class FlowInstanceDataProcessingServiceTests
{
    [Fact]
    public async Task ShouldAddAndUpdateFlowInstanceDataAsync()
    {
        // Given
        FlowInstanceData added = CreateRandomFlowInstanceData();
        added.Id = Guid.Empty;
        FlowInstanceData updated = CreateRandomFlowInstanceData();
        FlowInstanceData dbVersion = CreateRandomFlowInstanceData();
        dbVersion.Id = updated.Id;

        flowInstanceDataServiceMock
            .Setup(expression: service => service.AddFlowInstanceDataAsync(added))
            .ReturnsAsync(value: added);
        flowInstanceDataServiceMock
            .Setup(expression: service => service.Get(updated.Id))
            .Returns(value: dbVersion);
        flowInstanceDataServiceMock
            .Setup(expression: service => service.UpdateFlowInstanceDataAsync(dbVersion))
            .ReturnsAsync(value: dbVersion);

        // When
        Result<FlowInstanceData>[] results = (await flowInstanceDataProcessingService
            .AddOrUpdateFlowInstanceData(items: new[] { added, updated }))
            .ToArray();

        // Then
        results.Should().OnlyContain(predicate: result => result.Success);
        results[0].Message.Should().Be(expected: "Added Successfully");
        results[1].Message.Should().Be(expected: "Updated Successfully");
        flowInstanceDataServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldCaptureAddOrUpdateFlowInstanceDataFailureAsync()
    {
        // Given
        FlowInstanceData item = CreateRandomFlowInstanceData();
        item.Id = Guid.Empty;

        flowInstanceDataServiceMock
            .Setup(expression: service => service.AddFlowInstanceDataAsync(item))
            .Throws(exception: new Exception(message: "failed"));

        // When
        Result<FlowInstanceData> result = (await flowInstanceDataProcessingService
            .AddOrUpdateFlowInstanceData(items: new[] { item }))
            .Single();

        // Then
        result.Success.Should().BeFalse();
        result.Item.Should().BeSameAs(expected: item);
        result.Message.Should().Be(expected: "The Workflow service failed.");
    }

    [Fact]
    public async Task ShouldRejectMissingFlowInstanceDataWhenUpdatingAsync()
    {
        // Given
        FlowInstanceData item = CreateRandomFlowInstanceData();

        flowInstanceDataServiceMock
            .Setup(expression: service => service.Get(item.Id))
            .Returns(value: null);

        // When
        Func<Task> action = async () => await flowInstanceDataProcessingService
            .UpdateFlowInstanceDataAsync(updatedEntity: item);

        // Then
        await action.Should().ThrowAsync<System.Security.SecurityException>();
    }
}
#pragma warning restore STXFORMAT005, STXFORMAT009