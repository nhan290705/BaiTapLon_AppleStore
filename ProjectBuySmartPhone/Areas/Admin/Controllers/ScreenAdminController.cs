using Microsoft.AspNetCore.Mvc;

namespace ProjectBuySmartPhone.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ScreenAdminController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult DashBoard()
        {
            return View();
        }
            
    }
}
