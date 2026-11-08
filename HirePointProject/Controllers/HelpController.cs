using Microsoft.AspNetCore.Mvc;

namespace HirePoint.Controllers
{
    public class HelpController : Controller
    {
        // =========================================================
        // HELP CENTRE
        // =========================================================

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        // =========================================================
        // USER GUIDE / INTERACTIVE TOUR
        // =========================================================

        [HttpGet]
        public IActionResult UserGuide()
        {
            return View();
        }


        // =========================================================
        // FAQ
        // =========================================================

        [HttpGet]
        public IActionResult FAQs()
        {
            return View();
        }


        // =========================================================
        // TROUBLESHOOTING
        // =========================================================

        [HttpGet]
        public IActionResult Troubleshooting()
        {
            return View();
        }
    }
}

