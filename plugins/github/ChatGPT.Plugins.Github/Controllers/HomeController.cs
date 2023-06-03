using Microsoft.AspNetCore.Mvc;

namespace ChatGPT.Plugins.Github.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[ApiController]
public class HomeController : ControllerBase
{
    [HttpGet]
    [Route("/")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var html = await System.IO.File.ReadAllTextAsync(@"./wwwroot/index.html", cancellationToken);
        return Content(html, "text/html");
    }
}
