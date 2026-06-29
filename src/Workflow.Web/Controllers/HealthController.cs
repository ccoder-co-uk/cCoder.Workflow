using Microsoft.AspNetCore.Mvc;

namespace Workflow.Web.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Content("OK", "text/plain");
}
