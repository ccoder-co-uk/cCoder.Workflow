using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities;
using cCoder.Workflow.Activities.Models;
using JsonBroker = cCoder.Workflow.Brokers.JsonBroker;
using cCoder.Workflow.Services.Orchestrations;
using cCoder.Workflow.Services.Processings;
using FizzWare.NBuilder;
using Microsoft.Extensions.Logging;
using Moq;

namespace cCoder.Core.Services.Tests.Workflow.Orchestrations;

public partial class EventHandlingOrchestrationServiceTests
{
    private readonly Mock<IWorkflowEventProcessingService> workflowEventProcessingServiceMock;
    private readonly Mock<IFlowInstanceDataProcessingService> flowInstanceDataProcessingServiceMock;
    private readonly Mock<IFlowInstanceDataEventProcessingService> flowInstanceDataEventProcessingServiceMock;
    private readonly EventHandlingOrchestrationService orchestrationService;

    public EventHandlingOrchestrationServiceTests()
    {
        workflowEventProcessingServiceMock = new Mock<IWorkflowEventProcessingService>(MockBehavior.Strict);
        flowInstanceDataProcessingServiceMock = new Mock<IFlowInstanceDataProcessingService>(MockBehavior.Strict);
        flowInstanceDataEventProcessingServiceMock = new Mock<IFlowInstanceDataEventProcessingService>(MockBehavior.Strict);
        orchestrationService = new EventHandlingOrchestrationService(
            workflowEventProcessingServiceMock.Object,
            flowInstanceDataProcessingServiceMock.Object,
            flowInstanceDataEventProcessingServiceMock.Object,
            new JsonBroker(),
            Mock.Of<ILogger<EventHandlingOrchestrationService>>());
    }

    private static Page CreateRandomPage() =>
        Builder<Page>.CreateNew()
            .With(x => x.Id = 1)
            .With(x => x.AppId = 1)
            .With(x => x.Name = "Home")
            .With(x => x.Path = "home")
            .Build();

    private static WorkflowEvent CreateSubscription(Page page, string executeAs = "system") =>
        CreateSubscription(page, Guid.NewGuid(), executeAs);

    private static WorkflowEvent CreateSubscription(Page page, Guid flowId, string executeAs = "system") =>
        new()
        {
            Id = Guid.NewGuid(),
            Type = "Any",
            EventContext = $"page_update{page.Path}",
            FlowId = flowId,
            ExecuteAs = executeAs,
            ExecuteAsUser = new User { Id = executeAs },
            Flow = new FlowDefinition
            {
                Id = flowId,
                AppId = page.AppId,
                DefinitionJson = new JsonBroker().Serialize(new Flow
                {
                    Name = "Test Flow",
                    Activities = [new Start()],
                    Links = []
                })
            }
        };
}
