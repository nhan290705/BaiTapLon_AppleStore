using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBuySmartPhone.Models.Domain.Entities;
using ProjectBuySmartPhone.Models.Domain.Enums;
using ProjectBuySmartPhone.Responsitory;
using System;
using X.PagedList;
namespace ProjectBuySmartPhone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class CustomerController : Controller
    {
        private readonly IUserRepository _userRepository;

        public CustomerController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: Danh sách khách hàng
        [HttpGet]
        public IActionResult Index(string searchTerm, int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var query = _userRepository.GetAll()
                .Where(u => u.Role == "USER"); // Chỉ lấy khách hàng (RoleId = 2)

            // Tìm kiếm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchLower = searchTerm.ToLower();
                query = query.Where(u =>
                    (u.LastName != null && u.LastName.ToLower().Contains(searchLower)) ||
                    (u.Email != null && u.Email.ToLower().Contains(searchLower)) ||
                    (u.Username != null && u.Username.ToLower().Contains(searchLower)) ||
                    (u.PhoneNumber != null && u.PhoneNumber.ToLower().Contains(searchLower))
                );
            }

            query = query.OrderBy(u => u.UserId);

            ViewBag.SearchTerm = searchTerm;

            var pagedList = query.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        // GET: Chi tiết khách hàng
        [HttpGet]
        public IActionResult Details(int id)
        {
            var user = _userRepository.GetAll()
                //.Include(u => u.Role)
                .Include(u => u.Orders)
                .FirstOrDefault(u => u.UserId == id);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy khách hàng!";
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // GET: Sửa thông tin khách hàng
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _userRepository.GetAll()
                .FirstOrDefault(u => u.UserId == id);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy khách hàng!";
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // POST: Sửa thông tin khách hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User user)
        {
            // Remove password validation vì không cập nhật password ở đây
            ModelState.Remove("Password");

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            try
            {
                var existingUser = _userRepository.GetAll()
                    .FirstOrDefault(u => u.UserId == user.UserId);

                if (existingUser == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy khách hàng!";
                    return RedirectToAction(nameof(Index));
                }

                // Kiểm tra email trùng (ngoại trừ user hiện tại)
                var emailExists = _userRepository.GetAll()
                    .Any(u => u.Email == user.Email && u.UserId != user.UserId);

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng!");
                    return View(user);
                }

                // Kiểm tra username trùng (ngoại trừ user hiện tại)
                var usernameExists = _userRepository.GetAll()
                    .Any(u => u.Username == user.Username && u.UserId != user.UserId);

                if (usernameExists)
                {
                    ModelState.AddModelError("UserName", "Tên đăng nhập đã được sử dụng!");
                    return View(user);
                }

                // Cập nhật thông tin (không đổi password)
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;
                existingUser.Username = user.Username;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Address = user.Address;
                existingUser.UpdatedAt = DateTime.Now;

                _userRepository.update(existingUser);

                TempData["SuccessMessage"] = "Cập nhật thông tin khách hàng thành công!";
                return RedirectToAction(nameof(Details), new { id = user.UserId });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật!");
                return View(user);
            }
        }

        // GET: Đổi mật khẩu
        [HttpGet]
        public IActionResult ChangePassword(int id)
        {
            var user = _userRepository.GetAll()
                .FirstOrDefault(u => u.UserId == id);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy khách hàng!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.User = user;
            return View();
        }

        // POST: Đổi mật khẩu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(int userId, string newPassword, string confirmPassword)
        {
            var user = _userRepository.GetAll()
                .FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy khách hàng!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.User = user;

            // Validate
            if (string.IsNullOrEmpty(newPassword))
            {
                ModelState.AddModelError("newPassword", "Vui lòng nhập mật khẩu mới!");
                return View();
            }

            if (newPassword.Length < 6)
            {
                ModelState.AddModelError("newPassword", "Mật khẩu phải có ít nhất 6 ký tự!");
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("confirmPassword", "Mật khẩu xác nhận không khớp!");
                return View();
            }

            try
            {
                // Hash password với BCrypt
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

                user.Password = hashedPassword;
                user.UpdatedAt = DateTime.Now;

                _userRepository.update(user);

                TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                return RedirectToAction(nameof(Details), new { id = userId });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra khi đổi mật khẩu!");
                return View();
            }
        }

        // POST: Xóa khách hàng (soft delete hoặc hard delete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var user = _userRepository.GetAll()
                    .Include(u => u.Orders)
                    .FirstOrDefault(u => u.UserId == id);

                if (user == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy khách hàng!";
                    return RedirectToAction(nameof(Index));
                }

                // Kiểm tra xem user có đơn hàng không
                if (user.Orders != null && user.Orders.Any())
                {
                    TempData["ErrorMessage"] = $"Không thể xóa khách hàng vì có {user.Orders.Count} đơn hàng liên quan!";
                    return RedirectToAction(nameof(Details), new { id });
                }

                _userRepository.delete(user);

                TempData["SuccessMessage"] = "Xóa khách hàng thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa khách hàng!";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Tạo khách hàng mới
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tạo khách hàng mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user, string confirmPassword)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            try
            {
                // Kiểm tra email trùng
                var emailExists = _userRepository.GetAll()
                    .Any(u => u.Email == user.Email);

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng!");
                    return View(user);
                }

                // Kiểm tra username trùng
                var usernameExists = _userRepository.GetAll()
                    .Any(u => u.Username == user.Username);

                if (usernameExists)
                {
                    ModelState.AddModelError("UserName", "Tên đăng nhập đã được sử dụng!");
                    return View(user);
                }

                // Validate password
                if (string.IsNullOrEmpty(user.Password))
                {
                    ModelState.AddModelError("Password", "Vui lòng nhập mật khẩu!");
                    return View(user);
                }

                if (user.Password.Length < 6)
                {
                    ModelState.AddModelError("Password", "Mật khẩu phải có ít nhất 6 ký tự!");
                    return View(user);
                }

                if (user.Password != confirmPassword)
                {
                    ModelState.AddModelError("confirmPassword", "Mật khẩu xác nhận không khớp!");
                    return View(user);
                }

                // Hash password
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                user.Role = "USER" ; // Customer role
                user.CreatedAt = DateTime.Now;

                _userRepository.add(user);

                TempData["SuccessMessage"] = "Tạo tài khoản khách hàng thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra khi tạo tài khoản!");
                return View(user);
            }
        }
    }
}