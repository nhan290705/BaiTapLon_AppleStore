using Microsoft.AspNetCore.Mvc;
using ProjectBuySmartPhone.Dtos;
using ProjectBuySmartPhone.Helpers;
using ProjectBuySmartPhone.Models.Infrastructure;
namespace ProjectBuySmartPhone.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController>? _logger;
        private readonly MyDbContext? _context;
        private readonly JwtHelper _jwtHelper;
       public LoginController(ILogger<LoginController> logger, MyDbContext context, JwtHelper jwtHelper)
        {
            _logger = logger;
            _context = context;
            _jwtHelper = jwtHelper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(UserLogin userLogin)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u =>
                    u.Username.ToUpper() == userLogin.Username.ToUpper());
                if (user == null)
                {
                    ModelState.AddModelError("Username", "Username does not exist");
                    return View(ModelState);
                }
                if (!BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
                {
                    ModelState.AddModelError("Password", "Incorrect password");
                    return View(ModelState);
                }
                var token = _jwtHelper.GenerateTokens(user.UserId);
                Console.WriteLine("Generated Token: " + token.accessToken);
                Console.WriteLine("Generated Refresh Token: " + token.refreshToken);
                Response.Cookies.Append("AccessToken", token.accessToken);
                Response.Cookies.Append("RefreshToken", token.refreshToken);

                return RedirectToAction("Index", "Home", new {area = ""});
            }
            return View();
        }
        private IActionResult RedirectToHomeWithRole(string userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId.ToString() == userId);
            ViewBag.UserName = user?.Username;
            string roleName = _context.Users
                .Where(u => u.UserId.ToString() == userId)
                .Select(u => u.Role)
                .FirstOrDefault() ?? "";
            if (roleName.ToUpper() == "ADMIN")
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            if (roleName.ToUpper() == "USER")
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
