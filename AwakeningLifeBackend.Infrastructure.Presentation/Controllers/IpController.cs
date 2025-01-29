using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AwakeningLifeBackend.Infrastructure.Presentation.Controllers;

[ApiController]
[Route("api/ip")]
[Authorize]
public class IpController : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetIp()
    {
        HttpClient client = new HttpClient();

        string responseBody = await client.GetStringAsync("http://checkip.amazonaws.com/");

        return Ok(responseBody);
    }
}
