using Microsoft.AspNetCore.Mvc;
using ProjectBuySmartPhone.Dtos;
using ProjectBuySmartPhone.Models.Domain.Entities;
using ProjectBuySmartPhone.Models.Infrastructure;

namespace ProjectBuySmartPhone.Areas.Users.Controllers
{
    [Area("Users")] // ✅ PHẢI TRÙNG TÊN FOLDER "Users"
    public class UserController : Controller
    {
        private readonly MyDbContext _context;

        public UserController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(); 
        }

        private int? getCurrentUserId()
        {
            try
            {
                var accessToken = Request.Cookies["AccessToken"];
                if (string.IsNullOrEmpty(accessToken))
                    return null;

                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(accessToken);
                var idClaim = token.Claims.FirstOrDefault(c => c.Type == "idUser");

                if (idClaim != null && int.TryParse(idClaim.Value, out int userId))
                    return userId;

                return null;
            }
            catch
            {
                return null;
            }
        }

        // ----------------------- PROFILE ------------------------
        [HttpGet]
        public IActionResult Profile()
        {
            var userId = getCurrentUserId();
            if (userId == null)
                return RedirectToAction("Index", "Login", new { area = "Identity" });

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
                return NotFound();

            user.Email = MaskEmail(user.Email);
            user.PhoneNumber = MaskPhone(user.PhoneNumber);

            return View(user);
        }

        // ----------------------- EDIT ------------------------
        [HttpGet]
        public IActionResult Edit()
        {
            var userId = getCurrentUserId();
            if (userId == null)
                return RedirectToAction("Index", "Login", new { area = "Identity" });

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
                return NotFound();

            var model = new UserUpdate
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return View(model); // ✅ View nằm ở /Areas/Users/Views/User/Edit.cshtml
        }

        // ----------------------- UPDATE ------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateUser(UserUpdate update)
        {
            var userId = getCurrentUserId();
            if (userId == null) return RedirectToAction("Index", "Login", new { area = "Identity" });

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["ActiveTab"] = "Edit";
                return View("Edit", update);
            }

            // --- Cập nhật thông tin cơ bản ---
            if (!string.IsNullOrWhiteSpace(update.FirstName))
                user.FirstName = update.FirstName;

            if (!string.IsNullOrWhiteSpace(update.LastName))
                user.LastName = update.LastName;

            if (!string.IsNullOrWhiteSpace(update.Email))
                user.Email = update.Email;

            if (!string.IsNullOrWhiteSpace(update.PhoneNumber))
                user.PhoneNumber = update.PhoneNumber;

            // --- Đổi mật khẩu nếu có nhập ---
            if (!string.IsNullOrWhiteSpace(update.currentPassword) ||
                !string.IsNullOrWhiteSpace(update.newPassword) ||
                !string.IsNullOrWhiteSpace(update.confirmNewPassword))
            {
                if (string.IsNullOrWhiteSpace(update.currentPassword) ||
                    string.IsNullOrWhiteSpace(update.newPassword) ||
                    string.IsNullOrWhiteSpace(update.confirmNewPassword))
                {
                    ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin mật khẩu.");
                    ViewData["ActiveTab"] = "Edit";
                    return View("Edit", update);
                }

                if (!BCrypt.Net.BCrypt.Verify(update.currentPassword, user.Password))
                {
                    ModelState.AddModelError("currentPassword", "Mật khẩu hiện tại không đúng.");
                    ViewData["ActiveTab"] = "Edit";
                    return View("Edit", update);
                }

                if (update.newPassword != update.confirmNewPassword)
                {
                    ModelState.AddModelError("confirmNewPassword", "Mật khẩu xác nhận không khớp.");
                    ViewData["ActiveTab"] = "Edit";
                    return View("Edit", update);
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(update.newPassword);
            }

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Profile", "User", new { area = "Users" });
        }

        // ----------------------- MASK FUNCS ------------------------
        private string MaskPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone) || phone.Length < 4)
                return "***";
            return phone.Substring(0, 3) + new string('*', phone.Length - 3);
        }

        private string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
                return "***";
            var parts = email.Split('@');
            var name = parts[0];
            var domain = parts[1];
            string maskedName = name.Length <= 3 ? name.Substring(0, 1) + "***" : name.Substring(0, 3) + "***";
            return maskedName + "@" + domain;
        }
    }
}
