// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using cCoder.Workflow.Dependencies.OData;

namespace cCoder.Workflow.Extensions.OData;

internal static class TypeExtensions
{
    internal static string GetCSharpTypeName(this Type type)
    {
        if (!type.IsGenericType)
        {
            return type.Name;
        }

        IEnumerable<string> genericNames =
            type.GenericTypeArguments.Select(
                selector: argument => argument.GetCSharpTypeName());

        return $"{type.Name.Split(separator: '`')[0]}<{string.Join(separator: ",", values: genericNames)}>"
            .Replace(oldValue: "System.Object", newValue: "dynamic");
    }

    internal static bool IsJoinType(this Type type)
    {
        TableAttribute table = type.GetCustomAttribute<TableAttribute>();

        return table != null
            && type.GetProperties().Length == 4
            && type.GetProperties()
                .Where(predicate: property =>
                    property.PropertyType.IsValueType
                    || property.PropertyType == typeof(string))
                .All(predicate: property =>
                    property.GetCustomAttribute<ForeignKeyAttribute>() != null);
    }

    internal static PropertyInfo GetIdProperty(this Type type)
    {
        if (type.IsJoinType())
        {
            return new CompositePropertyInfo(type);
        }

        return type.GetProperty(name: "ID")
            ?? type.GetProperty(name: "Id")
            ?? type.GetProperty(name: type.Name + "Id")
            ?? type.GetProperty(name: type.Name + "ID")
            ?? type.GetProperties()
                .FirstOrDefault(predicate: property =>
                    property.GetCustomAttributes(
                        attributeType: typeof(KeyAttribute),
                        inherit: false)
                    .Any());
    }
}