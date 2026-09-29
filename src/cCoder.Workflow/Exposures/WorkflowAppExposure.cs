// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.CodeAnalysis.Exposures;
using cCoder.Data.Models.CMS;
using cCoder.Workflow.Services.Coordinations;

namespace cCoder.Workflow.Exposures;

internal class WorkflowAppExposure(IAppCoordinationService appCoordinationService)
    : IWorkflowAppExposure, ICompositionExposure
{
    public ValueTask AddAsync(App newApp) =>
        appCoordinationService.AddAppAsync(newApp: newApp);

    public ValueTask UpdateAsync(App updatedApp) =>
        appCoordinationService.UpdateAppAsync(updatedApp: updatedApp);

    public ValueTask DeleteAsync(int appId) =>
        appCoordinationService.DeleteAsync(appId: appId);
}