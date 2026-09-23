// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;

namespace cCoder.Workflow.Brokers;

internal sealed class ReflectionBroker
    : IReflectionBroker, IUtilityBroker
{
    public object GetPropertyValue(
        object instance,
        string propertyName) =>
        instance.GetType()
            .GetProperty(name: propertyName)?
            .GetValue(obj: instance);
}