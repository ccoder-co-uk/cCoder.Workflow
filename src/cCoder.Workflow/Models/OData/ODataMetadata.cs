// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Models.OData;

public class MetadataContainerSet
{
    public string Name { get; set; }

    public string UriBase { get; set; }

    public ExtendedMetadataContainer[] Types { get; set; }
}