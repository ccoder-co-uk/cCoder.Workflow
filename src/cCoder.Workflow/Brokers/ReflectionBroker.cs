// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;
using System.Reflection;

namespace cCoder.Workflow.Brokers;

internal sealed class ReflectionBroker
    : IReflectionBroker, IUtilityBroker
{
    public T GetCustomAttribute<T>(System.Reflection.MemberInfo member)
        where T : Attribute =>
        member.GetCustomAttribute<T>();

    public System.Reflection.PropertyInfo[] GetProperties(Type type) =>
        type.GetProperties();

    public object GetPropertyValue(
        object instance,
        string propertyName) =>
        instance.GetType()
            .GetProperty(name: propertyName)?
            .GetValue(obj: instance);
}