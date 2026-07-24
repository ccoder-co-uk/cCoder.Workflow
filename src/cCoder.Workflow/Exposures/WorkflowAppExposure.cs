// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Coordinations;

namespace cCoder.Workflow.Exposures;

internal class WorkflowAppExposure(IAppCoordinationService appCoordinationService)
    : IWorkflowAppExposure
{
    public ValueTask AddAsync(App newApp) =>
        appCoordinationService.AddAppAsync(newApp: newApp);

    public ValueTask UpdateAsync(App updatedApp) =>
        appCoordinationService.UpdateAppAsync(updatedApp: updatedApp);

    public ValueTask DeleteAsync(int appId) =>
        appCoordinationService.DeleteAsync(appId: appId);
}