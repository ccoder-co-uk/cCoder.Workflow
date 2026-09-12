// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace Workflow.Web.Brokers;

internal sealed class CoreAppBroker(ICoreContextFactory coreContextFactory)
    : ICoreAppBroker
{
    public async ValueTask<App> SelectAppAsync(int appId)
    {
        await using CoreDataContext coreDataContext =
            coreContextFactory.CreateCoreContext();

        return await coreDataContext.Set<App>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(predicate: app => app.Id == appId);
    }
}