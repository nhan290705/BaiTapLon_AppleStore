using Microsoft.AspNetCore.Mvc;
using ProjectBuySmartPhone.Dtos;
using ProjectBuySmartPhone.Models.Infrastructure;

namespace ProjectBuySmartPhone.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("[area]/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController>? _logger;
        private readonly MyDbContext? _context;
        public LoginController(ILogger<LoginController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpPost("login")]
        public IActionResult Login(UserLogin userLogin)
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

            }
            return Ok(new {message = "Login successfully"});
        }
    }
}
