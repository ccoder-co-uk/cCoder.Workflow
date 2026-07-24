// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace cCoder.Workflow.Api.OData;

internal static class ODataCollectionExtensions
{
    internal static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        if (source == null)
        {
            return;
        }

        foreach (T item in source)
        {
            action(obj: item);
        }
    }
}

internal static class ODataJsonExtensions
{
    internal static string ToJsonForOdata(this object value) =>
        JsonConvert.SerializeObject(value: value, formatting: Formatting.None, settings: GetODataJsonSettings());

    private static JsonSerializerSettings GetODataJsonSettings() =>
        new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
            Formatting = Formatting.None,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ContractResolver = new DefaultContractResolver { IgnoreSerializableAttribute = true },
            MaxDepth = 4,
        };
}

internal static class ODataTypeExtensions
{
    internal static string GetCSharpTypeName(this Type type)
    {
        if (!type.IsGenericType)
        {
            return type.Name;
        }

        IEnumerable<string> genericNames = type.GenericTypeArguments.Select(selector: argument => argument.GetCSharpTypeName());
        return $"{type.Name.Split(separator: '`')[0]}<{string.Join(separator: ",", values: genericNames)}>".Replace(oldValue: "System.Object", newValue: "dynamic");
    }

    internal static bool IsJoinType(this Type type)
    {
        TableAttribute table = type.GetCustomAttribute<TableAttribute>();

        return table != null
            && type.GetProperties().Length == 4
            && type.GetProperties()
                .Where(predicate: property => property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                .All(predicate: property => property.GetCustomAttribute<ForeignKeyAttribute>() != null);
    }

    internal static PropertyInfo GetIdProperty(this Type type)
    {
        if (!type.IsJoinType())
        {
            PropertyInfo idProperty =
                type.GetProperty(name: "ID")
                ?? type.GetProperty(name: "Id")
                ?? type.GetProperty(name: type.Name + "Id")
                ?? type.GetProperty(name: type.Name + "ID")
                ?? type.GetProperties()
                .FirstOrDefault(predicate: property =>
                    property.GetCustomAttributes(attributeType: typeof(KeyAttribute), inherit: false)
                .Any());

            if (idProperty != null)
            {
                return idProperty;
            }
        }
        else
        {
            return new CompositePropertyInfo(type);
        }

        return null;
    }
}

internal sealed class CompositePropertyInfo(Type type) : PropertyInfo
{
    public override PropertyAttributes Attributes => (PropertyAttributes)PropertyType.Attributes;
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
    public override object[] GetCustomAttributes(Type attributeType, bool inherit) =>
        PropertyType.GetCustomAttributes(attributeType: attributeType, inherit: inherit);
    public override MethodInfo GetGetMethod(bool nonPublic) =>
        throw new NotImplementedException();
    public override ParameterInfo[] GetIndexParameters() =>
        throw new NotImplementedException();
    public override MethodInfo GetSetMethod(bool nonPublic) =>
        throw new NotImplementedException();
    public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture) =>
        throw new NotImplementedException();
    public override bool IsDefined(Type attributeType, bool inherit) =>
        PropertyType.IsDefined(attributeType: attributeType, inherit: inherit);
    public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture) =>
        throw new NotImplementedException();
}