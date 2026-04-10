using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using FluentAssertions;
using Moq;
using Xunit;
using DataUser = cCoder.Data.Models.Security.User;


namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class FlowDefinitionOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCreateAndPersistFlowInstanceWhenQueueAsync()
    {
        // Given
        Guid id = Guid.NewGuid();
        DataUser user = TestUsers.WithPrivilege("flowdefinition_execute", 1);
        Guid queuedId = Guid.NewGuid();
        FlowDefinition flowDefinition = new()
        {
            Id = id,
            AppId = 1,
            App = new cCoder.Data.Models.CMS.App
            {
                Id = 1,
                Domain = "app.local",
                Name = "App"
            },
            DefinitionJson =
                "{\"Activities\":[{\"$type\":\"cCoder.Core.Objects.Workflow.Activities.Start, cCoder.Core.Objects\",\"Name\":\"Start\"}],\"Links\":[]}",
            ConfigJson = "{}"
        };

        flowDefinitionServiceMock.Setup(x => x.Get(id)).Returns(flowDefinition);
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(user);
        flowInstanceDataOrchestrationServiceMock
            .Setup(x => x.AddAsync(It.IsAny<FlowInstanceData>()))
            .ReturnsAsync((FlowInstanceData instance) =>
            {
                instance.Id = queuedId;
                return instance;
            });

        // When
        Guid result = await orchestrationService.QueueAsync(id, "{}");

        // Then
        result.Should().Be(queuedId);
        flowDefinitionServiceMock.Verify(x => x.Get(id), Times.Once);
        authorizationBrokerMock.Verify(x => x.GetCurrentUser(), Times.Once);
        flowInstanceDataOrchestrationServiceMock.Verify(
            x =>
                x.AddAsync(
                    It.Is<FlowInstanceData>(instance =>
                        instance.FlowDefinitionId == id
                        && instance.Caller == user.Id
                        && instance.State == "Queued"
                        && instance.ContextString.Contains("\"ExecutionState\":\"Queued\"")
                    )
                ),
            Times.Once
        );
        flowDefinitionProcessingServiceMock.VerifyNoOtherCalls();
        flowDefinitionEventProcessingServiceMock.VerifyNoOtherCalls();
        flowDefinitionServiceMock.VerifyNoOtherCalls();
        flowInstanceDataOrchestrationServiceMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}








