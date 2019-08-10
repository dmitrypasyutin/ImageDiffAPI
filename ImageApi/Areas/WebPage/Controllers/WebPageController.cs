using Microsoft.AspNetCore.Mvc;

namespace ImageDiff.Api.Areas.WebPage.Controllers
{
    [Area("WebPage")]
    public class WebPageController : Controller
    {
        public WebPageController()
        {
        }

        [HttpGet("sync")]
        public ViewResult Sync()
        {
            return View();
        }

        [HttpGet("async")]
        public ViewResult Async()
        {
            return View();
        }
    }
}