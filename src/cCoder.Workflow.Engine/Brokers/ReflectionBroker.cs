// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using cCoder.CodeAnalysis.Exposures;

namespace cCoder.Workflow.Engine.Brokers;

internal sealed class ReflectionBroker
    : IReflectionBroker,
      IUtilityBroker
{
    public bool HasAttribute<TAttribute>(PropertyInfo property)
        where TAttribute : Attribute =>
        property.GetCustomAttribute<TAttribute>() is not null;

    public void SetValue(
        PropertyInfo property,
        object instance,
        object value) =>
        property.SetValue(obj: instance, value: value);

    public bool IsGenericType(Type type) =>
        type.IsGenericType;

    public string GetTypeName(Type type) =>
        type.Name;

    public Type[] GetGenericTypeArguments(Type type) =>
        type.GenericTypeArguments;
}