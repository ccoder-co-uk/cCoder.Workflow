// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class FlowInstanceDataOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<FlowInstanceData> entities = new[] { CreateRandomFlowInstanceData() }.AsQueryable();

        flowInstanceDataProcessingServiceMock.Setup(expression: x => x.GetAll(ignoreFilters: true))
            .Returns(value: entities);

        // When
        IQueryable<FlowInstanceData> result = orchestrationService.GetAll(ignoreFilters: true);

        // Then
        result.Should()
            .BeSameAs(expected: entities);

        flowInstanceDataProcessingServiceMock.Verify(expression: x => x.GetAll(ignoreFilters: true), times: Times.Once);
        flowInstanceDataProcessingServiceMock.VerifyNoOtherCalls();
        flowInstanceDataEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}