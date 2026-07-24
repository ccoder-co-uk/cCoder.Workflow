// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Workflow.HostedServices.Services.Processings;

namespace Workflow.HostedServices.Controllers;

[ApiController]
public sealed class HomeController(
    IHomeProcessingService homeProcessingService)
    : ControllerBase
{
    [HttpGet("/")]
    public IActionResult Get() =>
        Content(
            content: homeProcessingService.GetHome(),
            contentType: "text/plain");
}