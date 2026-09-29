// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.Workflow.Models;


namespace cCoder.Workflow.Services.Aggregations;

internal interface IWorkflowMigrationAggregationService
{
    ValueTask ImportPackageWorkflowPackageAsync(int appId, WorkflowPackage workflowPackage);

    WorkflowPackage ExportPackage(int appId, string packageName);
}