using Microsoft.AspNetCore.Mvc;

namespace PowerTraderExam.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "healthy", time = DateTime.UtcNow });
}
