// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace cCoder.Workflow.Brokers;

internal interface IServiceScopeBroker
{
    Task RunScopedAsync<TService>(
        Func<TService, Task> operation)
        where TService : notnull;
}