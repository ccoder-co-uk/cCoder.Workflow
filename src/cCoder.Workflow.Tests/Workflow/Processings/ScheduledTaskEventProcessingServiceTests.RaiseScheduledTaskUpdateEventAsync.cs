// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Planning;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class ScheduledTaskEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseScheduledTaskUpdateEventAsync()
    {
        // Given
        ScheduledTask entity = CreateRandomScheduledTask();

        scheduledTaskEventServiceMock
            .Setup(expression: x => x.RaiseScheduledTaskUpdateEventAsync(scheduledTask: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseScheduledTaskUpdateEventAsync(scheduledTask: entity);

        // Then
        scheduledTaskEventServiceMock.Verify(expression: x => x.RaiseScheduledTaskUpdateEventAsync(scheduledTask: entity), times: Times.Once);
        scheduledTaskEventServiceMock.VerifyNoOtherCalls();
    }

}