// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Services.Foundations;


namespace cCoder.Workflow.Tests;

public partial class WorkflowMetadataTypeServiceTests
{
    private readonly IWorkflowMetadataTypeService service;

    public WorkflowMetadataTypeServiceTests() =>
        service = new WorkflowMetadataTypeService();
}