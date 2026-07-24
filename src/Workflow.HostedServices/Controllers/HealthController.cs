// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Workflow.HostedServices.Services.Processings;

namespace Workflow.HostedServices.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class HealthController(
    IHealthProcessingService healthProcessingService)
    : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Content(
            content: healthProcessingService.GetHealth(),
            contentType: "text/plain");
}