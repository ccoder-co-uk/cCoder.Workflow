// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers.OData;
using Microsoft.OData.ModelBuilder;

namespace cCoder.Workflow.Extensions.OData;

public static class ODataConventionModelBuilderExtensions
{
    public static void ConfigureWorkflowApiModel(
        this ODataConventionModelBuilder builder) =>
        new WorkflowModelBroker(builder: builder)
            .Configure();
}