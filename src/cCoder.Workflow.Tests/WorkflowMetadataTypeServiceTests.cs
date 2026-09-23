// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Services.Foundations;
using cCoder.Workflow.Brokers;


namespace cCoder.Workflow.Tests;

public partial class WorkflowMetadataTypeServiceTests
{
    private readonly IWorkflowMetadataTypeService service;

    public WorkflowMetadataTypeServiceTests() =>
        service = new WorkflowMetadataTypeService(
            reflectionBroker: new ReflectionBroker());
}