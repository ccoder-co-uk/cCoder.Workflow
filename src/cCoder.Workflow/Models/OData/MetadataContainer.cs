// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using cCoder.Workflow.Extensions.OData;

namespace cCoder.Workflow.Models.OData;

public class MetadataContainer
{
    public string Type { get; set; }
    public string ServerTypeName { get; set; }
    public bool IsValueType { get; set; }
    public bool IsEntity { get; set; }
    public bool IsJoinEntity { get; set; }
    public bool HasEndpoint { get; set; }
    public bool IsSystemManaged { get; set; }
    public string Category { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public string ServerType { get; set; }
    public IEnumerable<PropertyContainer> Properties { get; set; }

    public MetadataContainer() { }

    public MetadataContainer(Type type)
    {
        Dictionary<Type, string> typeNames = new()
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

        IsValueType = type.IsValueType || type == typeof(string);
        Type = type == typeof(string)
            ? "string"
            : typeof(IEnumerable).IsAssignableFrom(c: type)
                ? "array"
                : typeNames.TryGetValue(key: type, value: out string typeName)
                    ? typeName
                    : "object";
        Name = type.Name;
        DisplayName = type.Name;
        Description = type.Name;
        ServerType = type.AssemblyQualifiedName;
        ServerTypeName = type.GetCSharpTypeName();
        Properties = type.IsValueType || type == typeof(string)
            ? []
            : type.GetProperties()
            .Select(selector: property => new PropertyContainer
            {
                Name = property.Name,
                Type = property.PropertyType == typeof(string)
                    ? "string"
                    : typeof(IEnumerable).IsAssignableFrom(c: property.PropertyType)
                        ? "array"
                        : typeNames.TryGetValue(
                            key: property.PropertyType,
                            value: out string propertyTypeName)
                            ? propertyTypeName
                            : "object",
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
                        && property.PropertyType.GetGenericTypeDefinition()
                            == typeof(Nullable<>))
                    && property.PropertyType.IsValueType)
                    || property.GetCustomAttribute<RequiredAttribute>() is not null
            })
            .ToArray();
    }

    public MetadataContainer(Type type, bool isEntity, bool hasEndpoint)
        : this(type)
    {
        IsEntity = isEntity;
        IsJoinEntity = isEntity && type.IsJoinType();
        HasEndpoint = hasEndpoint;
    }

}