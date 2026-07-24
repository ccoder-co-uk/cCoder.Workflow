// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Dependencies.OData;


namespace cCoder.Workflow.Services.Foundations;

public interface IWorkflowMetadataTypeService
{
    MetadataContainerSet GetCoreMetadata();

    MetadataContainerSet[] GetKnownActivityTypes();

    MetadataContainerSet[] GetKnownSystemTypes();

    MetadataContainerSet GetSharedMetadata();
}