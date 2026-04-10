using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations.Events;
using cCoder.Workflow.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.Workflow.Processings;

public partial class WorkflowEventEventProcessingServiceTests
{
    private readonly Mock<IWorkflowEventEventService> workflowEventEventServiceMock;
    private readonly WorkflowEventEventProcessingService service;

    public WorkflowEventEventProcessingServiceTests()
    {
        workflowEventEventServiceMock = new Mock<IWorkflowEventEventService>(MockBehavior.Strict);
        service = new WorkflowEventEventProcessingService(workflowEventEventServiceMock.Object);
    }

    private static WorkflowEvent CreateRandomWorkflowEvent() =>
        Builder<WorkflowEvent>.CreateNew().Build();
}










