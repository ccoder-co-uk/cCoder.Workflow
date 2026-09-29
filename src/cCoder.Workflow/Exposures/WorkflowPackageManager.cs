// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.CodeAnalysis.Exposures;
using cCoder.Workflow.Models;
using cCoder.Workflow.Services.Aggregations;


namespace cCoder.Workflow.Exposures;

internal class WorkflowPackageManager(
    IWorkflowMigrationAggregationService workflowMigrationAggregationService
) : IWorkflowPackageManager, ICompositionExposure
{
    public ValueTask ImportPackageAsync(int appId, WorkflowPackage workflowPackage) =>
        workflowMigrationAggregationService.ImportPackageWorkflowPackageAsync(appId: appId, workflowPackage: workflowPackage);

    public WorkflowPackage ExportPackage(int appId, string packageName) =>
        workflowMigrationAggregationService.ExportPackage(appId: appId, packageName: packageName);
}