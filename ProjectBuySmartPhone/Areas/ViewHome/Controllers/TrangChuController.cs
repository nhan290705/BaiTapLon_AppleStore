using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBuySmartPhone.Models.Domain.Entities;
using ProjectBuySmartPhone.Models.Infrastructure;
using X.PagedList;

namespace ProjectBuySmartPhone.Areas.ViewHome.Controllers
{
    [Area("ViewHome")]
    public class TrangChuController : Controller
    {
        private readonly MyDbContext _db;
        private readonly IWebHostEnvironment _env;

        public TrangChuController(MyDbContext db)
        {
            this._db = db;
        }

        public IActionResult Index(string? keyword)
        {
            var list = _db.Products
                          .Include(p => p.ProductCategory)
                          .Include(p => p.ProductImages)
                          .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(p => p.ProductName.Contains(keyword) ||
                                       p.Description.Contains(keyword) ||
                                       p.Slug.Contains(keyword));
                ViewBag.Keyword = keyword;
            }

            return View("TrangChu", list.ToList());
        }

        public async Task<IActionResult> ListIphone(string? keyword, int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "Iphone");

            // 🔍 Nếu có từ khóa -> lọc theo tên hoặc mô tả
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.ProductName.Contains(keyword) ||
                                         p.Description.Contains(keyword) ||
                                         p.Slug.Contains(keyword));
                ViewBag.Keyword = keyword; // để hiển thị lại trên ô input
            }

            var pagedList = await query.OrderBy(p => p.ProductName)
                                       .ToPagedListAsync(page, pageSize);

            return View("ProductIphone", pagedList);
        }

        public async Task<IActionResult> InforIphone(int productId, int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "Iphone")
                           .OrderBy(p => p.ProductName); // ổn định thứ tự

            var pagedList = await query.ToPagedListAsync(page, pageSize);

            foreach (var a in query)
            {
                if (a.ProductId == productId)
                {
                    ViewBag.PP = a;
                    break;
                }
            }

            return View("ProductIphone", pagedList);
        }

        public async Task<IActionResult> ListMac(string? keyword, int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "Mac");

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.ProductName.Contains(keyword) ||
                                         p.Description.Contains(keyword) ||
                                         p.Slug.Contains(keyword));
                ViewBag.Keyword = keyword;
            }

            var pagedList = await query.OrderBy(p => p.ProductName)
                                       .ToPagedListAsync(page, pageSize);

            return View("ProductMac", pagedList);
        }
        public async Task<IActionResult> InforMac(int productId, int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "Mac")
                           .OrderBy(p => p.ProductName); // ổn định thứ tự

            var pagedList = await query.ToPagedListAsync(page, pageSize);

            foreach (var a in query)
            {
                if (a.ProductId == productId)
                {
                    ViewBag.PP = a;
                    break;
                }
            }

            return View("ProductMac", pagedList);
        }

        public async Task<IActionResult> ListIpad(string? keyword, int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "Ipad");

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.ProductName.Contains(keyword) ||
                                         p.Description.Contains(keyword) ||
                                         p.Slug.Contains(keyword));
                ViewBag.Keyword = keyword;
            }

            var pagedList = await query.OrderBy(p => p.ProductName)
                                       .ToPagedListAsync(page, pageSize);

            return View("ProductIpad", pagedList);
        }
        public async Task<IActionResult> InforIpad(int productId, int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "Ipad")
                           .OrderBy(p => p.ProductName); // ổn định thứ tự

            var pagedList = await query.ToPagedListAsync(page, pageSize);

            foreach (var a in query)
            {
                if (a.ProductId == productId)
                {
                    ViewBag.PP = a;
                    break;
                }
            }

            return View("ProductIpad", pagedList);
        }



        public async Task<IActionResult> ListAppleWatch(string? keyword, int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "Apple Watch");

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.ProductName.Contains(keyword) ||
                                         p.Description.Contains(keyword) ||
                                         p.Slug.Contains(keyword));
                ViewBag.Keyword = keyword;
            }

            var pagedList = await query.OrderBy(p => p.ProductName)
                                       .ToPagedListAsync(page, pageSize);

            return View("ProductAppleWatch", pagedList);
        }
        public async Task<IActionResult> InforAppleWatch(int productId, int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "Apple Watch")
                           .OrderBy(p => p.ProductName); // ổn định thứ tự

            var pagedList = await query.ToPagedListAsync(page, pageSize);

            foreach (var a in query)
            {
                if (a.ProductId == productId)
                {
                    ViewBag.PP = a;
                    break;
                }
            }

            return View("ProductAppleWatch", pagedList);
        }

        public async Task<IActionResult> ListAccessory(string? keyword, int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "Accessory");

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.ProductName.Contains(keyword) ||
                                         p.Description.Contains(keyword) ||
                                         p.Slug.Contains(keyword));
                ViewBag.Keyword = keyword;
            }

            var pagedList = await query.OrderBy(p => p.ProductName)
                                       .ToPagedListAsync(page, pageSize);

            return View("ProductAccessory", pagedList);
        }
        public async Task<IActionResult> InforAccessory(int productId, int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "Accessory")
                           .OrderBy(p => p.ProductName); // ổn định thứ tự

            var pagedList = await query.ToPagedListAsync(page, pageSize);

            foreach (var a in query)
            {
                if (a.ProductId == productId)
                {
                    ViewBag.PP = a;
                    break;
                }
            }

            return View("ProductAccessory", pagedList);
        }
        [HttpGet]
        public IActionResult Search(string keyword, string? category)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return RedirectToAction("Index");
            }

            var query = _db.Products
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .AsQueryable();

            // Nếu có category -> lọc theo danh mục
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.ProductCategory.CategoryName == category);
            }

            // Lọc theo từ khóa (theo tên sản phẩm, mô tả, slug)
            query = query.Where(p =>
                p.ProductName.Contains(keyword) ||
                p.Description.Contains(keyword) ||
                p.Slug.Contains(keyword));

            var results = query.ToList();

            ViewBag.Keyword = keyword;
            ViewBag.Category = category ?? "Tất cả sản phẩm";

            return View("SearchResult", results);
        }

    }
}
