// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;

namespace cCoder.Workflow.Exposures;

public interface IWorkflowAppExposure
{
    ValueTask AddAsync(App newApp);

    ValueTask UpdateAsync(App updatedApp);

    ValueTask DeleteAsync(int appId);
}