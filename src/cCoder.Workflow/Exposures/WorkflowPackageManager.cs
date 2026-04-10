using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Aggregations;


namespace cCoder.Workflow.Exposures;

internal class WorkflowPackageManager(
    IWorkflowMigrationAggregationService workflowMigrationAggregationService
) : IWorkflowPackageManager
{
    public ValueTask ImportPackageAsync(int appId, WorkflowPackage package) =>
        workflowMigrationAggregationService.ImportPackageAsync(appId, package);

    public WorkflowPackage ExportPackage(int appId, string packageName) =>
        workflowMigrationAggregationService.ExportPackage(appId, packageName);
}


