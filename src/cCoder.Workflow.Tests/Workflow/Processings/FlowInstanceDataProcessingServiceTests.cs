using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations;
using cCoder.Workflow.Services.Processings;
using Moq;
using DataUser = cCoder.Data.Models.Security.User;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class FlowInstanceDataProcessingServiceTests
{
    private readonly Mock<IFlowInstanceDataService> flowInstanceDataServiceMock = new();
    private DataUser currentUser = TestUsers.WithoutPrivileges();
    private readonly FlowInstanceDataProcessingService flowInstanceDataProcessingService;

    public FlowInstanceDataProcessingServiceTests()
    {
        flowInstanceDataProcessingService = new FlowInstanceDataProcessingService(
            flowInstanceDataServiceMock.Object
        );
    }

    private static FlowInstanceData CreateRandomFlowInstanceData() =>
        new()
        {
            Id = Guid.NewGuid(),
            FlowDefinitionId = Guid.NewGuid(),
            Name = "Instance",
            State = "Queued",
            ContextString = "{}",
            FlowDefinition = new FlowDefinition
            {
                Id = Guid.NewGuid(),
                AppId = 1,
                Name = "Flow",
            },
        };
}












