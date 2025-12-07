using System.Diagnostics;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Sentry;

namespace backend.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        // test sentry
        public IActionResult TestSentry()
        {
            SentrySdk.CaptureMessage("This is a test message from HomeController.TestSentry");
            return Content("Sentry test message sent.");
		}
    }
}
