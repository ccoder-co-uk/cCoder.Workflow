// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Services.Coordinations;

public interface IAppCoordinationService
{
    ValueTask AddAppAsync(App newApp);

    ValueTask UpdateAppAsync(App updatedApp);

    ValueTask DeleteAsync(int appId);
}
