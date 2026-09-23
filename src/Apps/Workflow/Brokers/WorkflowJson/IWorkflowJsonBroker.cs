// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace Workflow.Brokers.WorkflowJson;

internal interface IWorkflowJsonBroker
{
    T Deserialize<T>(string value);
}