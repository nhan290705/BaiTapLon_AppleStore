using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectBuySmartPhone.Dtos;
using ProjectBuySmartPhone.Helpers;
using ProjectBuySmartPhone.Models.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
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
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
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
                    return View(userLogin);
                }
                if (!BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
                {
                    ModelState.AddModelError("Password", "Incorrect password");
                    return View(userLogin);
                }
                var isHttps = HttpContext.Request.IsHttps;
                var token = _jwtHelper.GenerateTokens(user.UserId);
                var tokenHandler = new JwtSecurityTokenHandler();
                var accessJwt = tokenHandler.ReadJwtToken(token.accessToken);
                var refreshJwt = tokenHandler.ReadJwtToken(token.refreshToken);
                Console.WriteLine("Generated Token: " + token.accessToken);
                Console.WriteLine("Generated Refresh Token: " + token.refreshToken);
                // add token vao cookie
                Response.Cookies.Append("AccessToken", token.accessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = accessJwt.ValidTo
                });

                Response.Cookies.Append("RefreshToken", token.refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = refreshJwt.ValidTo
                });
                Console.WriteLine("login thanh cong");
                return RedirectToHomeWithRole(user.UserId.ToString());
            }
            return View(userLogin);
        }
        // Redirect user to home page based on their role
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
                return RedirectToAction("Index", "DashBoard", new { area = "Admin" });
            }
            if (roleName.ToUpper() == "USER")
            {
                return RedirectToAction("Index", "TrangChu", new { area = "ViewHome" });
            }
            return RedirectToAction("Index", "TrangChu", new { area = "ViewHome" });
        }
        public IActionResult AccessDenied()
        {
                       return View();
        }
    }
}
