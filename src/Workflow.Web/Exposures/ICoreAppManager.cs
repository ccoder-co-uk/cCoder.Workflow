// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.Data.Models.CMS;

namespace Workflow.Web.Exposures;

public interface ICoreAppManager
{
    ValueTask<App> GetAppAsync(int appId);
}