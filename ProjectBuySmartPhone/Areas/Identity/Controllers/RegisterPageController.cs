using Microsoft.AspNetCore.Mvc;

namespace ProjectBuySmartPhone.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("Identity/Register")]
    public class RegisterPageController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View("~/Areas/Identity/Views/Register/Index.cshtml");
        }
    }
}
