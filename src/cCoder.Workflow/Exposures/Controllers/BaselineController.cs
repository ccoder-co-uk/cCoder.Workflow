// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Exposures.Setup;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Workflow.Exposures.Controllers;

[ApiController]
[Route("Api/Workflow/Baseline")]
public sealed class BaselineController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(value:UIBaseline.Packages);
}