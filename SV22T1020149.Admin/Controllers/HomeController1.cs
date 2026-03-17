using Microsoft.AspNetCore.Mvc;

namespace SV22T1020149.Admin.Controllers
{
    public class HomeController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
