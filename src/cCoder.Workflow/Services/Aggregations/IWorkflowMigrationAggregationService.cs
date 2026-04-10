using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Aggregations;

public interface IWorkflowMigrationAggregationService
{
    ValueTask ImportPackageAsync(int appId, WorkflowPackage package);

    WorkflowPackage ExportPackage(int appId, string packageName);
}



