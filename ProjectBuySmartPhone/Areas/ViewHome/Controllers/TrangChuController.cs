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
        public IActionResult Index()
        {
            List<Product> listIphone = _db.Products
                                       .Include(p => p.ProductCategory)
                                       .Include(p => p.ProductImages)
                                       .Where(p => p.ProductCategory.CategoryName == "Iphone")
                                       .ToList();
            return View("TrangChu",listIphone);
        }
        public async Task<IActionResult> ListIphone(int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "Iphone")
                           .OrderBy(p => p.ProductName); // ổn định thứ tự

            var pagedList = await query.ToPagedListAsync(page, pageSize);

          

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

        public async Task<IActionResult> ListMac(int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "Mac")
                           .OrderBy(p => p.ProductName); // ổn định thứ tự

            var pagedList = await query.ToPagedListAsync(page, pageSize);



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

        public async Task<IActionResult> ListIpad(int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "Ipad")
                           .OrderBy(p => p.ProductName); // ổn định thứ tự

            var pagedList = await query.ToPagedListAsync(page, pageSize);



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



        public async Task<IActionResult> ListAppleWatch(int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "AppleWatch")
                           .OrderBy(p => p.ProductName); // ổn định thứ tự

            var pagedList = await query.ToPagedListAsync(page, pageSize);



            return View("ProductAppleWatch", pagedList);
        }
        public async Task<IActionResult> InforAppleWatch(int productId, int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "AppleWatch")
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

        public async Task<IActionResult> ListAccessory(int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "Accessory")
                           .OrderBy(p => p.ProductName); // ổn định thứ tự

            var pagedList = await query.ToPagedListAsync(page, pageSize);



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
    }
}
