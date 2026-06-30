using Microsoft.AspNetCore.Mvc;

namespace Workflow.HostedServices.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Content("OK", "text/plain");
}
