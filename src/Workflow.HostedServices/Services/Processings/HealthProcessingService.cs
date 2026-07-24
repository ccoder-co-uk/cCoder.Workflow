// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace Workflow.HostedServices.Services.Processings;

internal sealed partial class HealthProcessingService
    : IHealthProcessingService
{
    public string GetHealth() =>
        TryCatch(operation: () => "OK");
}