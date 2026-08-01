// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using cCoder.Workflow.Extensions.OData;

namespace cCoder.Workflow.Models.OData;

public class PropertyContainer
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string ServerType { get; set; }
    public string ServerTypeName { get; set; }
    public string Template { get; set; }
    public string DisplayName { get; set; }
    public string ShortDisplayName { get; set; }
    public string Description { get; set; }
    public bool IsGeneric { get; set; }
    public bool IsValueType { get; set; }
    public bool IsReadOnly { get; set; }
    public bool IsRequired { get; set; }
    public bool IsSystemManaged { get; set; }
}