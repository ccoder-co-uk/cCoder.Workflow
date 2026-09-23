// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers;

namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class WorkflowConfigurationService(
    IWorkflowConfigurationBroker configurationBroker)
    : IWorkflowConfigurationService
{
    public string GetServiceUrl() =>
        TryCatch(operation: () =>
        {
            return configurationBroker.GetServiceUrl();
        });
}