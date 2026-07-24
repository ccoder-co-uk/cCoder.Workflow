// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Orchestrations;

namespace cCoder.Workflow.Exposures;

internal class WorkflowAppExposure(IAppOrchestrationService appOrchestrationService)
    : IWorkflowAppExposure
{
    public ValueTask AddAsync(App app) => appOrchestrationService.AddAsync(app);
    public ValueTask UpdateAsync(App app) => appOrchestrationService.UpdateAsync(app);
    public ValueTask DeleteAsync(int appId) => appOrchestrationService.DeleteAsync(appId);
}