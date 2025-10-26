using Microsoft.AspNetCore.Mvc;

namespace ProjectBuySmartPhone.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("Identity/Login")]
    public class LoginPageController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Areas/Identity/Views/Login/Index.cshtml");
        }
    }
}
