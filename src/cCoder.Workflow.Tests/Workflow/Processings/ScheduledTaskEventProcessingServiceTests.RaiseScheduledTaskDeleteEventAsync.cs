// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Planning;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class ScheduledTaskEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughCallWhenRaiseScheduledTaskDeleteEventAsync()
    {
        // Given
        ScheduledTask entity = CreateRandomScheduledTask();

        scheduledTaskEventServiceMock
            .Setup(expression: x => x.RaiseScheduledTaskDeleteEventAsync(entity: entity))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseScheduledTaskDeleteEventAsync(entity: entity);

        // Then
        scheduledTaskEventServiceMock.Verify(expression: x => x.RaiseScheduledTaskDeleteEventAsync(entity: entity), times: Times.Once);
        scheduledTaskEventServiceMock.VerifyNoOtherCalls();
    }

}