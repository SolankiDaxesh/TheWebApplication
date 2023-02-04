using Microsoft.AspNetCore.Mvc;

namespace TheWebApplication.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
