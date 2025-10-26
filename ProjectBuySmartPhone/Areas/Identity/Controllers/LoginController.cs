using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

<<<<<<< HEAD:ProjectBuySmartPhone/Areas/Identity/Controller/LoginController.cs
using ProjectBuySmartPhone.Dtos;
using ProjectBuySmartPhone.Helpers;
using ProjectBuySmartPhone.Models.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;

=======
>>>>>>> c542b8e52931e409b1110dcccca2f4115cbb3de8:ProjectBuySmartPhone/Areas/Identity/Controllers/LoginController.cs
namespace ProjectBuySmartPhone.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("[area]/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
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


        [HttpPost("login")]
        public IActionResult Index(UserLogin userLogin)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u =>
                    u.Username.ToUpper() == userLogin.Username.ToUpper());
                if (user == null)
                {
                    ModelState.AddModelError("Username", "Username does not exist");
                    return BadRequest(ModelState);
                }
                if (!BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
                {
                    ModelState.AddModelError("Password", "Incorrect password");
                    return BadRequest(ModelState);
                }
                var token = _jwtHelper.GenerateTokens(user.UserId);
                Console.WriteLine("Generated Token: " + token.accessToken);

            }

            return Ok(new {message = "Login successfully"});
        }
        //private IActionResult RedirectToHomeWithRole(string userId)
        //{
        //    var user = _context.Users.FirstOrDefault(u => u.UserId.ToString() == userId);
        //    ViewBag.UserName = user?.Username;
        //    string roleName = _context.Users
        //        .Where(u => u.UserId.ToString() == userId)
        //        .Select(u => u.Role)
        //        .FirstOrDefault() ?? "";
        //    if(roleName.ToUpper() == "ADMIN")
        //    {
        //        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        //    }
        //    if(roleName.ToUpper() == "USER")
        //    {
        //        return RedirectToAction("Index", "Home", new { area = "" });
        //    }
        //    return RedirectToAction("Index", "Home", new { area = "" });    
        //}
    }
}
