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
}