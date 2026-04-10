using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class FlowInstanceDataProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAddAsync()
    {
        // Given
        FlowInstanceData entity = CreateRandomFlowInstanceData();
        flowInstanceDataServiceMock.Setup(x => x.AddAsync(entity)).ReturnsAsync(entity);

        // When
        FlowInstanceData result = await flowInstanceDataProcessingService.AddAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        flowInstanceDataServiceMock.Verify(x => x.AddAsync(entity), Times.Once);
        flowInstanceDataServiceMock.VerifyNoOtherCalls();
    }

}







