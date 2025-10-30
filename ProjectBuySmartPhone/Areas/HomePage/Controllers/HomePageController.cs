using Microsoft.AspNetCore.Mvc;

namespace ProjectBuySmartPhone.Areas.HomePage.Controllers
{
    [Area("HomePage")]
    public class HomePageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
