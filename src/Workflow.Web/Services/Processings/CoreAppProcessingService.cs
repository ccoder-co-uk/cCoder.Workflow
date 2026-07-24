// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace Workflow.Web.Services.Processings;

internal sealed partial class CoreAppProcessingService(
    ICoreContextFactory coreContextFactory)
    : ICoreAppProcessingService
{
    public ValueTask<App> GetAppAsync(int appId) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [appId]);

            await using CoreDataContext core =
                coreContextFactory.CreateCoreContext();

            return await core.Set<App>()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                    predicate: found => found.Id == appId);
        });
}