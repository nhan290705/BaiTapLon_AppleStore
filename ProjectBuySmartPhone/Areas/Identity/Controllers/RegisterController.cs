using Microsoft.AspNetCore.Mvc;
using ProjectBuySmartPhone.Dtos;
using ProjectBuySmartPhone.Models.Domain.Entities;
using ProjectBuySmartPhone.Models.Infrastructure;
using BCrypt.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;

namespace ProjectBuySmartPhone.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class RegisterController : Controller
    {
        private readonly ILogger<RegisterController> _logger;
        private readonly MyDbContext _context;

        public RegisterController(ILogger<RegisterController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Index(UserRegister userRegister)
        {
            if(!ModelState.IsValid)
            {
                return View(userRegister);
            }   
            var existingUser = _context.Users.FirstOrDefault(u => u.Username == userRegister.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "Username already exists");
                return View(userRegister);
            }
            existingUser = _context.Users.FirstOrDefault(u => u.Email == userRegister.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email already exists");
                return View(userRegister);
            }
            existingUser = _context.Users.FirstOrDefault(u => u.PhoneNumber == userRegister.PhoneNumber);  
            if (existingUser != null)
            {
                ModelState.AddModelError("PhoneNumber", "Phone number already exists");
                return View(userRegister);
            }
            User newUser = userRegister.ToUser();
            newUser.Password = BCrypt.Net.BCrypt.HashPassword(userRegister.Password);
            _context.Users.Add(newUser);
            _context.SaveChanges();
            _logger.LogInformation($"New user registered : {newUser.Username}");
            TempData["RegisterSuccess"] = "Đăng ký tài khoản thành công! Chào mừng bạn đến với Apple Store 🍎";
            return RedirectToAction("Index", "Home", new {area = ""});
        }
    }
}
