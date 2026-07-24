// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Exposures;

public interface IWorkflowAppExposure
{
    ValueTask AddAsync(App newApp);

    ValueTask UpdateAsync(App updatedApp);

    ValueTask DeleteAsync(int appId);
}