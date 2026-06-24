using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations;
using cCoder.Workflow.Services.Processings;
using FizzWare.NBuilder;
using Moq;
using IAuthorizationBroker = cCoder.Workflow.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class WorkflowEventProcessingServiceTests
{
    private readonly Mock<IWorkflowEventService> workflowEventServiceMock = new();
    private readonly Mock<IFlowDefinitionService> flowDefinitionServiceMock = new();
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();
    private readonly WorkflowEventProcessingService workflowEventProcessingService;

    public WorkflowEventProcessingServiceTests()
    {
        workflowEventProcessingService = new WorkflowEventProcessingService(
            workflowEventServiceMock.Object,
            flowDefinitionServiceMock.Object,
            authorizationBrokerMock.Object
        );
    }

    private static WorkflowEvent CreateRandomWorkflowEvent() =>
        Builder<WorkflowEvent>
            .CreateNew()
            .With(x => x.Id = Guid.NewGuid())
            .With(x => x.FlowId = Guid.NewGuid())
            .With(x => x.ExecuteAs = Guid.NewGuid().ToString())
            .Build();
}

















