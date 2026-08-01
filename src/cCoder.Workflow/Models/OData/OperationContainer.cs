// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using cCoder.Workflow.Extensions.OData;

namespace cCoder.Workflow.Models.OData;

public class OperationContainer
{
    public string Name { get; set; }
    public string Url { get; set; }
    public string Definition { get; set; }
    public string HttpVerb { get; set; }
    public bool Queryable { get; set; }
    public MetadataContainer ReturnType { get; set; }
    public IDictionary<string, string> Parameters { get; set; }
}