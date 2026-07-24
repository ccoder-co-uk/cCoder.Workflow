// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace Workflow.Web.Services.Processings;

public interface ICoreAppProcessingService
{
    ValueTask<App> GetAppAsync(int appId);
}