// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Dependencies.ServiceProviders;

namespace cCoder.Workflow.Brokers.ServiceProviders;

internal interface IFlowDefinitionServiceProviderBroker
{
    T GetOperationService<T>(FlowDefinitionOperation operation)
        where T : notnull;
}