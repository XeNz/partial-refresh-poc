using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace TestPartialRender.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly ViewRendererService _viewRendererService;

    public HomeController(ILogger<HomeController> logger, ViewRendererService viewRendererService)
    {
        _logger = logger;
        _viewRendererService = viewRendererService;
    }

    [Produces(typeof(string))]
    public async Task<ContentResult> Index()
    {
        var str = await _viewRendererService.RenderPartialViewToString("Test");
        _logger.LogInformation(str);

        return new ContentResult
        {
            ContentType = "text/html",
            StatusCode = (int)HttpStatusCode.OK,
            Content = str
        };
    }

}