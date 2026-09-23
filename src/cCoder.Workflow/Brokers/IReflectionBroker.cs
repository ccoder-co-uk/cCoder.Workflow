// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Brokers;

internal interface IReflectionBroker
{
    T GetCustomAttribute<T>(System.Reflection.MemberInfo member)
        where T : Attribute;

    System.Reflection.PropertyInfo[] GetProperties(Type type);

    object GetPropertyValue(
        object instance,
        string propertyName);
}