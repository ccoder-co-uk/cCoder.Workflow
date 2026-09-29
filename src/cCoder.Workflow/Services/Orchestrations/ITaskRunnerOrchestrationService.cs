// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace cCoder.Workflow.Services.Orchestrations;

internal interface ITaskRunnerOrchestrationService
{
    Task RunContinuouslyAsync(CancellationToken cancellationToken = default);

    Task RunAsync(CancellationToken cancellationToken = default);
}