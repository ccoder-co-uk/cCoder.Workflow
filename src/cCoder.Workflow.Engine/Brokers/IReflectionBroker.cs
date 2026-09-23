// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;

namespace cCoder.Workflow.Engine.Brokers;

internal interface IReflectionBroker
{
    bool HasAttribute<TAttribute>(PropertyInfo property)
        where TAttribute : Attribute;

    void SetValue(
        PropertyInfo property,
        object instance,
        object value);

    bool IsGenericType(Type type);

    string GetTypeName(Type type);

    Type[] GetGenericTypeArguments(Type type);
}