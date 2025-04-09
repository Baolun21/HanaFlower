using Microsoft.AspNetCore.Mvc;

namespace BookShop.Controllers
{
    public class ThanhToanController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
