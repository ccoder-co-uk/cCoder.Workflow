// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Dependencies.ServiceProviders;

namespace cCoder.Workflow.Brokers.ServiceProviders;

internal sealed class FlowDefinitionServiceProviderBroker(IServiceProvider serviceProvider)
    : IFlowDefinitionServiceProviderBroker
{
    public T GetOperationService<T>(FlowDefinitionOperation operation)
        where T : notnull =>
        serviceProvider.GetRequiredKeyedService<T>(serviceKey: operation);
}