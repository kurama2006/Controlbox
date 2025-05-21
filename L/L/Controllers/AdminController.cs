using Microsoft.AspNetCore.Mvc;

namespace L.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
