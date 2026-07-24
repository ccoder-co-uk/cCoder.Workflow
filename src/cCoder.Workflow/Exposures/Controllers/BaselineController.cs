// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Services.Processings;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Workflow.Exposures.Controllers;

[ApiController]
[Route("Api/Workflow/Baseline")]
public sealed class BaselineController(IBaselineProcessingService baselineProcessingService) : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(value: baselineProcessingService.GetBaselinePackages());
}