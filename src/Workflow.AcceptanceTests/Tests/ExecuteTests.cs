using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Exposures;
using Moq;
using Newtonsoft.Json;
using Workflow.AcceptanceTests.Infrastructure;

namespace Workflow.AcceptanceTests.Tests;

public sealed partial class ExecuteTests
{
    private readonly Mock<IFlowRunner> flowRunnerMock = new();
    private readonly Execute function;

    public ExecuteTests() =>
        function = new Execute(flowRunnerMock.Object);

    private static TestHttpRequestData CreateRequest(WorkflowRequest request) =>
        new(JsonConvert.SerializeObject(request));
}
