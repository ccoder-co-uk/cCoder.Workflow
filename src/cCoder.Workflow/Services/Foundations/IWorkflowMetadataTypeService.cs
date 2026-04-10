using cCoder.Workflow.Api.OData;


namespace cCoder.Workflow.Services.Foundations;

public interface IWorkflowMetadataTypeService
{
    MetadataContainerSet GetCoreMetadata();
    MetadataContainerSet[] GetKnownActivityTypes();
    MetadataContainerSet[] GetKnownSystemTypes();
    MetadataContainerSet GetSharedMetadata();
}


