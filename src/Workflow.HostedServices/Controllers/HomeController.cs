// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Workflow.HostedServices.Services.Processings;

using Workflow.HostedServices.Exposures;

namespace Workflow.HostedServices.Controllers;

[ApiController]
public sealed class HomeController(
    IHomeManager homeProcessingService)
    : ControllerBase
{
    [HttpGet("/")]
    public IActionResult Get() =>
        Content(
            content: homeProcessingService.GetHome(),
            contentType: "text/plain");
}