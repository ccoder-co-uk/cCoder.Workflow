// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Globalization;
using System.Reflection;

namespace cCoder.Workflow.Dependencies.OData;

internal sealed class CompositePropertyInfo(Type type) : PropertyInfo
{
    public override PropertyAttributes Attributes =>
        (PropertyAttributes)PropertyType.Attributes;
    public override bool CanRead => true;
    public override bool CanWrite => false;
    public override Type PropertyType { get; } = type;
    public override Type DeclaringType => PropertyType;
    public override string Name => PropertyType.Name;
    public override Type ReflectedType => PropertyType.ReflectedType;

    public override MethodInfo[] GetAccessors(bool nonPublic) =>
        throw new NotImplementedException();
    public override object[] GetCustomAttributes(bool inherit) =>
        PropertyType.GetCustomAttributes(inherit: inherit);
    public override object[] GetCustomAttributes(
        Type attributeType,
        bool inherit) =>
        PropertyType.GetCustomAttributes(
            attributeType: attributeType,
            inherit: inherit);
    public override MethodInfo GetGetMethod(bool nonPublic) =>
        throw new NotImplementedException();
    public override ParameterInfo[] GetIndexParameters() =>
        throw new NotImplementedException();
    public override MethodInfo GetSetMethod(bool nonPublic) =>
        throw new NotImplementedException();
    public override object GetValue(
        object obj,
        BindingFlags invokeAttr,
        Binder binder,
        object[] index,
        CultureInfo culture) =>
        throw new NotImplementedException();
    public override bool IsDefined(Type attributeType, bool inherit) =>
        PropertyType.IsDefined(
            attributeType: attributeType,
            inherit: inherit);
    public override void SetValue(
        object obj,
        object value,
        BindingFlags invokeAttr,
        Binder binder,
        object[] index,
        CultureInfo culture) =>
        throw new NotImplementedException();
}