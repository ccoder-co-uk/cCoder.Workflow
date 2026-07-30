// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Extensions.OData;
using cCoder.Workflow.Models.OData;


namespace cCoder.Workflow.Exposures;

public interface IWorkflowMetadataTypeManager
{
    MetadataContainerSet GetCoreMetadata();

    MetadataContainerSet[] GetKnownActivityTypes();

    MetadataContainerSet[] GetKnownSystemTypes();

    MetadataContainerSet GetSharedMetadata();
}