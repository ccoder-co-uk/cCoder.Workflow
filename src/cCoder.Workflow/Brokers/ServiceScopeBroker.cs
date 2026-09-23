// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Workflow.Brokers;

internal sealed class ServiceScopeBroker(
    IServiceScopeFactory serviceScopeFactory)
    : IServiceScopeBroker, IUtilityBroker
{
    public async Task RunScopedAsync<TService>(
        Func<TService, Task> operation)
        where TService : notnull
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();

        TService service =
            scope.ServiceProvider.GetRequiredService<TService>();

        await operation(arg: service);
    }
}