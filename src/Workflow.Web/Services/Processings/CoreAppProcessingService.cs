// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using Workflow.Web.Brokers;

namespace Workflow.Web.Services.Processings;

internal sealed partial class CoreAppProcessingService(
    ICoreAppBroker coreAppBroker)
    : ICoreAppProcessingService
{
    public ValueTask<App> GetAppAsync(int appId) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [appId]);

            return await coreAppBroker.SelectAppAsync(appId: appId);
        });
}