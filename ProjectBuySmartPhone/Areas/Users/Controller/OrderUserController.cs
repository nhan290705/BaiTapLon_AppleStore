using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBuySmartPhone.Models.Infrastructure;
using X.PagedList;

namespace ProjectBuySmartPhone.Areas.Users.Controllers
{
    [Area("Users")]
    public class OrderUserController : Controller
    {
        private readonly ILogger<OrderUserController> _logger;
        private MyDbContext _context;
        public OrderUserController(ILogger<OrderUserController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        private int? GetCurrentUserId()
        {
            try
            {
                var accessToken = Request.Cookies["AccessToken"];
                if (string.IsNullOrEmpty(accessToken)) return null;

                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(accessToken);
                var idClaim = token.Claims.FirstOrDefault(c => c.Type == "idUser");
                if (idClaim != null && int.TryParse(idClaim.Value, out int userId))
                    return userId;
                return null;
            }
            catch { return null; }
        }
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập.";
                return RedirectToAction("Index", "Login", new { area = "Identity" });
            }

            // Chỉ lấy đơn của user hiện tại. Nếu bạn muốn chỉ “đã xác nhận”
            // (ví dụ StatusOrderId = 1), thêm Where(o => o.StatusOrderId == 1)
            var query = _context.Orders
                .AsNoTracking()
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderId);

            var paged = await query.ToPagedListAsync(page, pageSize);
            return View(paged);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập.";
                return RedirectToAction("Index", "Login", new { area = "Identity" });
            }

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
            var order = _context.Orders
    .Include(o => o.OrderDetails)
        .ThenInclude(od => od.ProductDetails)
            .ThenInclude(pd => pd.Product)
    .FirstOrDefault(o => o.OrderId == id);

#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

            if (order == null)
                return NotFound();

            return View(order);
        }
    }
}
