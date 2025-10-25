using Microsoft.AspNetCore.Mvc;
using ProjectBuySmartPhone.Dtos;
using ProjectBuySmartPhone.Models.Domain.Entities;
using ProjectBuySmartPhone.Models.Infrastructure;
using BCrypt.Net;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ProjectBuySmartPhone.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("[area]/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly ILogger<RegisterController> _logger;
        private readonly MyDbContext _context;

        public RegisterController(ILogger<RegisterController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        [HttpPost]
        [Route("register")]
        public IActionResult Register(UserRegister userRegister)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }   
            var existingUser = _context.Users.FirstOrDefault(u => u.Username == userRegister.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "Username already exists");
                return BadRequest(ModelState);
            }
            existingUser = _context.Users.FirstOrDefault(u => u.Email == userRegister.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email already exists");
                return BadRequest(ModelState);
            }
            existingUser = _context.Users.FirstOrDefault(u => u.PhoneNumber == userRegister.PhoneNumber);  
            if (existingUser != null)
            {
                ModelState.AddModelError("PhoneNumber", "Phone number already exists");
                return BadRequest(ModelState);
            }
            User newUser = userRegister.ToUser();
            newUser.Password = BCrypt.Net.BCrypt.HashPassword(userRegister.Password);
            _context.Users.Add(newUser);
            _context.SaveChanges();
            _logger.LogInformation($"New user registered : {newUser.Username}");
            return Ok(new {message = "register successfully"});
        }
    }
}
