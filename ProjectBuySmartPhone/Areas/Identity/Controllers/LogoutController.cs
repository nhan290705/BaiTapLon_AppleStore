using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProjectBuySmartPhone.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class LogoutController : Controller
    {
        private readonly ILogger<LogoutController> _logger;
        public LogoutController(ILogger<LogoutController> logger)
        {
            _logger = logger;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            HttpContext.Response.Cookies.Delete("AccessToken", new CookieOptions
            {
                Path= "/"
            });
            HttpContext.Response.Cookies.Delete("RefreshToken", new CookieOptions
            {
                Path= "/"
            });
            _logger.LogInformation("Da logout");
            return RedirectToAction("Index", "Login", new { area = "Identity" });
        }
    }
}
