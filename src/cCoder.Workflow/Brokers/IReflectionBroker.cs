// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Brokers;

internal interface IReflectionBroker
{
    object GetPropertyValue(
        object instance,
        string propertyName);
}