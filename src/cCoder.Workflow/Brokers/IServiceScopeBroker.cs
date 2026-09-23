// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Brokers;

internal interface IServiceScopeBroker
{
    Task RunScopedAsync<TService>(
        Func<TService, Task> operation)
        where TService : notnull;
}