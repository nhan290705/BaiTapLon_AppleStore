using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ProjectBuySmartPhone.Models.Infrastructure;
using ProjectBuySmartPhone.Models.Domain.Entities;
using ProjectBuySmartPhone.Models.Domain.Enums;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProjectBuySmartPhone.Controllers
{
    [Area("Blog")]
    public class BlogController : Controller
    {
        private readonly MyDbContext _db;

        public BlogController(MyDbContext db)
        {
            _db = db;
        }

        // --- helpers ---
        private int? CurrentUserId()
        {
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(sub, out var id)) return id;

            var uid = User.FindFirst("uid")?.Value;
            if (int.TryParse(uid, out var id2)) return id2;

            return null;
        }

        // ========== LIST THREADS ==========
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? search, string? filter)
        {
            var query = _db.Blogs
                .Include(b => b.User)
                .Include(b => b.BlogComments)
                .Where(b => b.Status == BlogStatus.Published
                         || b.Status == BlogStatus.Solved
                         || b.Status == BlogStatus.Unsolved)
                .AsQueryable();

            // Lọc theo từ khóa
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b =>
                    b.Title.Contains(search) ||
                    b.Content.Contains(search));
            }

            // Lọc theo bộ lọc bên sidebar
            if (!string.IsNullOrEmpty(filter))
            {
                switch (filter.ToLower())
                {
                    case "popular":
                        // giả sử bài có nhiều bình luận là "popular"
                        query = query.OrderByDescending(b => b.BlogComments.Count);
                        break;
                    case "solved":
                        query = query.Where(b => b.Status == BlogStatus.Solved);
                        break;
                    case "unsolved":
                        query = query.Where(b => b.Status == BlogStatus.Unsolved);
                        break;
                }
            }

            var threads = await query
                .OrderByDescending(b => b.UpdatedAt)
                .ThenByDescending(b => b.CreatedAt)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.Filter = filter;

            return View("Index", threads);
        }



        // ========== THREAD DETAILS ==========
        [HttpGet("Details/{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var blog = await _db.Blogs
                .Include(b => b.User)
                .Include(b => b.BlogComments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(b => b.BlogId == id);

            if (blog == null) return NotFound();

            return View("Details", blog);
        }

        // ========== CREATE THREAD ==========
        //[HttpPost("Create")]
        //[Authorize]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([FromForm] ForumCreateDto dto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        TempData["Error"] = "Vui lòng điền đầy đủ thông tin bài viết.";
        //        return View("Create", dto);
        //    }

        //    var userId = CurrentUserId();
        //    if (userId == null)
        //    {
        //        TempData["Error"] = "Bạn cần đăng nhập để đăng bài.";
        //        return RedirectToAction(nameof(Index));
        //    }

        //    var blog = new Blog
        //    {
        //        Title = dto.Title.Trim(),
        //        Content = dto.Content?.Trim() ?? string.Empty,
        //        Status = BlogStatus.Published,
        //        UserId = userId.Value,
        //        CreatedAt = DateTime.Now,
        //        UpdatedAt = DateTime.Now
        //    };

        //    _db.Blogs.Add(blog);
        //    await _db.SaveChangesAsync();

        //    TempData["Success"] = "Đăng bài thành công!";
        //    return RedirectToAction(nameof(Details), new { id = blog.BlogId });
        //}

        //test luôn kh cần login
        [HttpPost("Create")]
        [AllowAnonymous] // ✅ Cho phép test
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ForumCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng điền đầy đủ thông tin bài viết.";
                return View("Create", dto);
            }

            // ✅ Gán tạm userId test
            int userId = 1;

            var blog = new Blog
            {
                Title = dto.Title.Trim(),
                Content = dto.Content?.Trim() ?? string.Empty,
                Status = BlogStatus.Published,
                UserId = userId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _db.Blogs.Add(blog);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Đăng bài thành công!";
            return RedirectToAction(nameof(Details), new { id = blog.BlogId });
        }

        // ========== DELETE THREAD ==========
        [HttpPost("Delete/{id:int}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _db.Blogs.FindAsync(id);
            if (blog == null)
            {
                TempData["Error"] = "Bài viết không tồn tại.";
                return RedirectToAction(nameof(Index));
            }

            var userId = CurrentUserId();
            if (userId == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập để thực hiện hành động này.";
                return RedirectToAction(nameof(Index));
            }

            // ✅ Chỉ cho phép người tạo hoặc admin xóa
            var user = await _db.Users.FindAsync(userId.Value);
            bool isOwner = blog.UserId == userId.Value;
            bool isAdmin = user != null && user.Role?.ToLower() == "admin";

            if (!isOwner && !isAdmin)
            {
                TempData["Error"] = "Bạn không có quyền xóa bài viết này.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // ✅ Xóa tất cả comment trước khi xóa bài viết
            var comments = _db.BlogComments.Where(c => c.BlogId == blog.BlogId);
            _db.BlogComments.RemoveRange(comments);

            // ✅ Xóa bài viết chính
            _db.Blogs.Remove(blog);
            await _db.SaveChangesAsync();

            // ✅ Sau khi xóa, quay lại trang Forum (Index)
            TempData["Success"] = "Xóa bài viết thành công!";
            return RedirectToAction("Index", "Forum");


            TempData["Success"] = "Xóa bài viết thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ========== SHOW CREATE FORM ==========
        [HttpGet("Create")]
        [AllowAnonymous] // ✅ tạm thời bỏ qua đăng nhập
        public IActionResult Create()
        {
            return View("Create", new Blog());
        }

        // ========== ADD COMMENT ==========
        [HttpPost("AddComment")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment([FromForm] ForumAddCommentDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Nội dung bình luận không hợp lệ.";
                return RedirectToAction(nameof(Details), new { id = dto.BlogId });
            }

            var userId = CurrentUserId();
            if (userId == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập.";
                return RedirectToAction(nameof(Details), new { id = dto.BlogId });
            }

            var exists = await _db.Blogs.AnyAsync(b => b.BlogId == dto.BlogId);
            if (!exists) return NotFound();

            var cmt = new BlogComment
            {
                BlogId = dto.BlogId,
                UserId = userId.Value,
                Message = dto.Message, // ✅ đúng với model BlogComment
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _db.BlogComments.Add(cmt);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = dto.BlogId });
        }
    }

    // ======= DTOs (tạm ở đây, có thể chuyển sang thư mục Dtos/Forum) =======
    public class ForumCreateDto
    {
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.MaxLength(220)]
        public string Title { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string Content { get; set; }
    }

    public class ForumAddCommentDto
    {
        [System.ComponentModel.DataAnnotations.Required]
        public int BlogId { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.MaxLength(1000)]
        public string Message { get; set; } 
    }
}
