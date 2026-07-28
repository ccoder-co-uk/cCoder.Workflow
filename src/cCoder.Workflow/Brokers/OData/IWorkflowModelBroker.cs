// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models.OData;

namespace cCoder.Workflow.Brokers.OData;

internal interface IWorkflowModelBroker
{
    ODataModel Build();

    void Configure();
}
