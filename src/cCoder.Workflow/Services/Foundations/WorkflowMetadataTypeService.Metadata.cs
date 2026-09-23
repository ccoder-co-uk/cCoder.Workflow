// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using cCoder.Workflow.Models.OData;

namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class WorkflowMetadataTypeService
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

    private ExtendedMetadataContainer CreateExtendedMetadataContainer(
        Type type,
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

    private string GetCSharpTypeName(Type type)
    {
        if (!type.IsGenericType)
        {
            return type.Name;
        }

        IEnumerable<string> genericNames =
            type.GenericTypeArguments.Select(
                selector: GetCSharpTypeName);

        return $"{type.Name.Split(separator: '`')[0]}<{string.Join(separator: ",", values: genericNames)}>"
            .Replace(oldValue: "System.Object", newValue: "dynamic");
    }

    private bool IsJoinType(Type type)
    {
        TableAttribute table = reflectionBroker.GetCustomAttribute<TableAttribute>(
            member: type);

        return table != null
            && reflectionBroker.GetProperties(type: type).Length == 4
            && reflectionBroker.GetProperties(type: type)
                .Where(predicate: property =>
                    property.PropertyType.IsValueType
                    || property.PropertyType == typeof(string))
                .All(predicate: property =>
                    reflectionBroker.GetCustomAttribute<ForeignKeyAttribute>(
                        member: property) != null);
    }

    private void PopulateMetadataContainer(
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
        metadata.ServerTypeName = GetCSharpTypeName(type: type);
        metadata.IsEntity = isEntity;
        metadata.IsJoinEntity = isEntity && IsJoinType(type: type);
        metadata.HasEndpoint = hasEndpoint;

        metadata.Properties = type.IsValueType || type == typeof(string)
            ? []
            : reflectionBroker.GetProperties(type: type)
                .Select(selector: CreatePropertyContainer)
                .ToArray();
    }

    private PropertyContainer CreatePropertyContainer(PropertyInfo property) =>
        new()
        {
            Name = property.Name,
            Type = GetClientType(type: property.PropertyType),
            ServerType = property.PropertyType.ToString(),
            ServerTypeName = GetCSharpTypeName(type: property.PropertyType),
            IsValueType = property.PropertyType.IsValueType
                || property.PropertyType == typeof(string),
            DisplayName = property.Name,
            ShortDisplayName = property.Name,
            Description = property.Name,
            IsReadOnly = !property.CanWrite,
            Template = reflectionBroker.GetCustomAttribute<KeyAttribute>(
                member: property) is not null
                || property.Name == "Id"
                    ? "key"
                    : property.Name,
            IsRequired = (!(property.PropertyType.IsGenericType
                    && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                && property.PropertyType.IsValueType)
                || reflectionBroker.GetCustomAttribute<RequiredAttribute>(
                    member: property) is not null
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