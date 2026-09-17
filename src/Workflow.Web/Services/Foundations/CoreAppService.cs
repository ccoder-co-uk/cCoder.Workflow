// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using Workflow.Web.Brokers;

namespace Workflow.Web.Services.Foundations;

internal sealed partial class CoreAppService(
    ICoreAppBroker coreAppBroker)
    : ICoreAppService
{
    public ValueTask<App> GetAppAsync(int appId) =>
        TryCatch(operation: async () =>
        {
            ValidateAppOnGet(inputs: [appId]);

            return await coreAppBroker.SelectAppAsync(appId: appId);
        });
}