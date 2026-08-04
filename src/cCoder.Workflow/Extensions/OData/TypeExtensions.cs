// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using cCoder.Workflow.Dependencies.OData;
using cCoder.Workflow.Models.OData;

namespace cCoder.Workflow.Extensions.OData;

internal static class TypeExtensions
{
    private static readonly IReadOnlyDictionary<Type, string> TypeNames =
        new Dictionary<Type, string>
        {
            { typeof(short), "number" }, { typeof(int), "number" },
            { typeof(long), "number" }, { typeof(short?), "number" },
            { typeof(int?), "number" }, { typeof(long?), "number" },
            { typeof(ushort), "number" }, { typeof(uint), "number" },
            { typeof(ulong), "number" }, { typeof(ushort?), "number" },
            { typeof(uint?), "number" }, { typeof(ulong?), "number" },
            { typeof(byte), "number" }, { typeof(byte?), "number" },
            { typeof(decimal), "number" }, { typeof(decimal?), "number" },
            { typeof(string), "string" }, { typeof(DateTime), "date" },
            { typeof(DateTime?), "date" }, { typeof(TimeSpan), "time" },
            { typeof(TimeSpan?), "time" }, { typeof(DateTimeOffset), "date" },
            { typeof(DateTimeOffset?), "date" }, { typeof(Guid), "guid" },
            { typeof(Guid?), "guid" }, { typeof(bool), "bool" },
            { typeof(bool?), "bool" }, { typeof(double), "number" },
            { typeof(double?), "number" }, { typeof(float), "number" },
            { typeof(float?), "number" }
        };

    internal static MetadataContainer CreateMetadataContainer(
        this Type type,
        bool isEntity = false,
        bool hasEndpoint = false)
    {
        MetadataContainer metadata = new();

        PopulateMetadataContainer(
            metadata: metadata,
            type: type,
            isEntity: isEntity,
            hasEndpoint: hasEndpoint);

        return metadata;
    }

    internal static ExtendedMetadataContainer CreateExtendedMetadataContainer(
        this Type type,
        bool isEntity = false,
        bool hasEndpoint = false)
    {
        ExtendedMetadataContainer metadata = new() { Operations = [] };

        PopulateMetadataContainer(
            metadata: metadata,
            type: type,
            isEntity: isEntity,
            hasEndpoint: hasEndpoint);

        return metadata;
    }

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

    private static void PopulateMetadataContainer(
        MetadataContainer metadata,
        Type type,
        bool isEntity,
        bool hasEndpoint)
    {
        metadata.IsValueType = type.IsValueType || type == typeof(string);
        metadata.Type = GetClientType(type: type);
        metadata.Name = type.Name;
        metadata.DisplayName = type.Name;
        metadata.Description = type.Name;
        metadata.ServerType = type.AssemblyQualifiedName;
        metadata.ServerTypeName = type.GetCSharpTypeName();
        metadata.IsEntity = isEntity;
        metadata.IsJoinEntity = isEntity && type.IsJoinType();
        metadata.HasEndpoint = hasEndpoint;

        metadata.Properties = type.IsValueType || type == typeof(string)
            ? []
            : type.GetProperties()
                .Select(selector: property => CreatePropertyContainer(property: property))
                .ToArray();

    }

    private static PropertyContainer CreatePropertyContainer(PropertyInfo property) =>
        new()
        {
            Name = property.Name,
            Type = GetClientType(type: property.PropertyType),
            ServerType = property.PropertyType.ToString(),
            ServerTypeName = property.PropertyType.GetCSharpTypeName(),
            IsValueType = property.PropertyType.IsValueType
                || property.PropertyType == typeof(string),
            DisplayName = property.Name,
            ShortDisplayName = property.Name,
            Description = property.Name,
            IsReadOnly = !property.CanWrite,
            Template = property.GetCustomAttribute<KeyAttribute>() is not null
                || property.Name == "Id"
                    ? "key"
                    : property.Name,
            IsRequired = (!(property.PropertyType.IsGenericType
                    && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                && property.PropertyType.IsValueType)
                || property.GetCustomAttribute<RequiredAttribute>() is not null
        };

    private static string GetClientType(Type type) =>
        type == typeof(string)
            ? "string"
            : typeof(IEnumerable).IsAssignableFrom(c: type)
                ? "array"
                : TypeNames.TryGetValue(key: type, value: out string typeName)
                    ? typeName
                    : "object";
}