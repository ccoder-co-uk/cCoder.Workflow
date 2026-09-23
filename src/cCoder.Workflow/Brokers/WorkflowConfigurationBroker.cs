// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;

namespace cCoder.Workflow.Brokers;

internal sealed class WorkflowConfigurationBroker(
    WorkflowConfiguration configuration)
    : IWorkflowConfigurationBroker
{
    public string GetServiceUrl() =>
        configuration.ServiceUrl;
}