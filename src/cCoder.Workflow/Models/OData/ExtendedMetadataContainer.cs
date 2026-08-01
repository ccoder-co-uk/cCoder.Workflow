// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using cCoder.Workflow.Extensions.OData;

namespace cCoder.Workflow.Models.OData;

public class ExtendedMetadataContainer : MetadataContainer
{
    public IEnumerable<OperationContainer> Operations { get; set; }

    public ExtendedMetadataContainer() { }

    public ExtendedMetadataContainer(Type type, bool isEntity = false, bool hasEndpoint = false)
        : base(type, isEntity, hasEndpoint) { }
}