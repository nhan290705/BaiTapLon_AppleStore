using Microsoft.AspNetCore.Mvc;
using ProjectBuySmartPhone.Dtos;
using ProjectBuySmartPhone.Models.Domain.Entities;
using ProjectBuySmartPhone.Models.Infrastructure;

namespace ProjectBuySmartPhone.Areas.Users.Controllers
{
    [Area("User")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly MyDbContext _context;
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Profile()
        {
            var userId = getCurrentUserId();
            if(userId == null) {
                return RedirectToAction("Index", "Login", new { area = "Identity" });
            }
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        private int? getCurrentUserId()
        {
            try
            {
                var accessToken = Request.Cookies["accessToken"];
                if(string.IsNullOrEmpty(accessToken))
                {
                    Console.WriteLine("Access token is null or empty");
                    return null;
                }
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(accessToken);
                var idClaim = token.Claims.FirstOrDefault(c => c.Type == "idUser");
                if(idClaim != null && int.TryParse(idClaim.Value, out int userId))
                {
                    return userId;
                }
                else
                {
                    Console.WriteLine("User ID claim is missing or invalid");
                    return null;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error retrieving current user ID: " + ex.Message);
                return null;
            }
        }
        [HttpPost]
        public IActionResult UpdateUser(UserUpdate update)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "idUser");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            int userId = int.Parse(userIdClaim.Value);
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound();
            }
            user.FirstName = update.FirstName;
            user.LastName = update.LastName;
            user.Email = update.Email;
            user.PhoneNumber = update.PhoneNumber;
            if(!string.IsNullOrEmpty(update.currentPassword) && !string.IsNullOrEmpty(update.newPassword))
            {
                if(!BCrypt.Net.BCrypt.Verify(update.currentPassword, user.Password))
                {
                    ModelState.AddModelError("currentPassword", "Current password is incorrect");
                    return View(update);
                }
                if (update.newPassword != update.confirmNewPassword)
                {
                    ModelState.AddModelError("confirmNewPassword", "New password and confirm password do not match");
                    return View(update);
                }
                if(update.currentPassword == update.newPassword)
                {
                    ModelState.AddModelError("newPassword", "New password must be different from current password");
                    return View(update);
                }
                user.Password = BCrypt.Net.BCrypt.HashPassword(update.newPassword);
            }
            _context.SaveChanges();
            Console.WriteLine("User updated successfully");

            return View(update);
        }
        //Thay doi address

    }
}
