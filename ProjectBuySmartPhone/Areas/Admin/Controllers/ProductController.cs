using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectBuySmartPhone.Areas.Admin.Models.ViewModels;
using ProjectBuySmartPhone.Dtos.Products;
using ProjectBuySmartPhone.Models.Domain.Entities;
using ProjectBuySmartPhone.Models.Domain.Enums;
using ProjectBuySmartPhone.Models.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace ProjectBuySmartPhone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    public class ProductController : Controller
    {
        private readonly MyDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ProductController(MyDbContext db, IWebHostEnvironment env)
        {
            this._db = db;
            _env = env;
        }

        // AJAX paging + filter theo CategoryName
        [HttpGet]
        public async Task<IActionResult> Mac(string? category = null, int page = 1)
        {
            const int pageSize = 10;

            // Lấy danh sách category cho dropdown
            var categories = await _db.ProductCategories
                                      .Select(c => c.CategoryName)
                                      .Distinct()
                                      .OrderBy(x => x)
                                      .ToListAsync();

            // MẶC ĐỊNH: iPhone nếu không truyền category
            var effectiveCategory = !string.IsNullOrWhiteSpace(category)
                ? category
                : "Iphone";

            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = effectiveCategory;

            // Map partial theo category (ở đây map TẤT CẢ về cùng 1 partial cho đơn giản)
            var partialName = GetPartialByCategory(effectiveCategory);
            ViewBag.PartialName = partialName;

            // Query đúng theo category đang chọn
            var query = _db.Products
                           .Include(p => p.ProductCategory)
                           .AsNoTracking()
                           .Where(p => p.ProductCategory.CategoryName == effectiveCategory)
                           .OrderBy(p => p.ProductName);

            var paged = await query.ToPagedListAsync(page, pageSize);

            // AJAX → chỉ trả về partial
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView(partialName, paged);

            // Non-AJAX → trả View đầy đủ
            return View("ProductMac", paged);
        }

        [HttpPost] // vẫn là POST  /Product/Search/123 -> Path Variable -> From  route productId=123
        // Product/Search?cateogry=abc&price=123  -> query param -> category=abc price=123      
        //Body Post put  delete. ->  
        // From Header 
                   // [ValidateAntiForgeryToken] // nếu bật, nhớ gửi token từ JS (xem dưới)
        public async Task<IActionResult> Search([FromQuery] string? category, [FromBody] SearchProductRequest request)
        {
            const int pageSize = 10;

            var effectiveCategory = !string.IsNullOrWhiteSpace(category) ? category : "Iphone";
            ViewBag.SelectedCategory = effectiveCategory;

            var partialName = GetPartialByCategory(effectiveCategory);

            var query = _db.Products
                           .Include(p => p.ProductCategory)
                           .AsNoTracking()
                           .Where(p => p.ProductCategory.CategoryName == effectiveCategory);

            if (!string.IsNullOrWhiteSpace(request.KeyWord))
            {
                var kw = $"%{request.KeyWord.Trim()}%";
                query = query.Where(p => EF.Functions.Like(p.ProductName, kw)
                                     || (p.Description != null && EF.Functions.Like(p.Description, kw)));
            }
            if (request.MinPrice.HasValue) query = query.Where(p => p.Price >= request.MinPrice.Value);
            if (request.MaxPrice.HasValue) query = query.Where(p => p.Price <= request.MaxPrice.Value);
            if (request.MinQuantity.HasValue) query = query.Where(p => p.Qty >= request.MinQuantity.Value);
            if (request.MaxQuantity.HasValue) query = query.Where(p => p.Qty <= request.MaxQuantity.Value);

            var pageIndex = request.PageIndex <= 0 ? 1 : request.PageIndex;

            // QUAN TRỌNG: partial của bạn cần IPagedList<Product>
            var paged = await query.OrderBy(p => p.ProductName)
                                   .ToPagedListAsync(pageIndex, pageSize);

            // Trả về đúng partial table + pager của bạn
            return PartialView(partialName, paged);
        }

        // 👉 Map tất cả category về 1 partial chung (_ProductMacList)
        private static string GetPartialByCategory(string? category)
        {
            return "_ProductMacList";
        }



        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            var categories = _db.ProductCategories
                .ToList();
            ViewBag.Categories = categories;
            return View(new ProductCreateVM());
        }

        // -------- CREATE (POST) --------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateVM vm)
        {
            var categories = _db.ProductCategories
                .ToList();
            ViewBag.Categories = categories;
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            // 1) Lưu Product
            var product = new Product
            {
                ProductName = vm.ProductName,
                Slug = vm.Slug,
                Description = vm.Description,
                Price = vm.Price,
                Qty = vm.Qty,
                Discount = vm.Discount,
                Color = vm.Color,
                Storage = vm.Storage,
                Ram = vm.Ram,
                Port = vm.Port,
                Status = (ActiveStatus)vm.Status,
                ProductCategoryId = vm.ProductCategoryId,
                CreatedAt = DateTime.UtcNow
            };
            _db.Products.Add(product);
            await _db.SaveChangesAsync(); // có ProductId

            // 2) Lưu ProductDetail
            var detail = new ProductDetail
            {
                ProductId = product.ProductId,
                Sku = vm.Sku,
                CreatedAt = DateTime.UtcNow
            };
            _db.ProductDetails.Add(detail);

            // 3) Lưu NHIỀU ảnh
            if (vm.ImageFiles != null && vm.ImageFiles.Count > 0)
            {
                var folder = Path.Combine(_env.WebRootPath, "images", "products");
                Directory.CreateDirectory(folder);

                // Nếu người dùng không chọn index ảnh chính, dùng file đầu tiên (0)
                var mainIndex = (vm.MainImageIndex >= 0 && vm.MainImageIndex < vm.ImageFiles.Count)
                                ? vm.MainImageIndex
                                : 0;

                for (int i = 0; i < vm.ImageFiles.Count; i++)
                {
                    var file = vm.ImageFiles[i];
                    if (file == null || file.Length == 0) continue;

                    var ext = Path.GetExtension(file.FileName);
                    var fileName = $"Img_Product_{product.ProductId}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{i}{ext}";
                    var filePath = Path.Combine(folder, fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // LƯU DB CHỈ TÊN FILE
                    _db.ProductImages.Add(new ProductImage
                    {
                        ProductId = product.ProductId,
                        ImageUrl = fileName,          // <-- chỉ tên file
                        IsMain = (i == mainIndex),  // một ảnh chính
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Mac");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int productId)
        {
            var categories = _db.ProductCategories
                .ToList();
            ViewBag.Categories = categories;
            var product = await _db.Products
                .AsNoTracking()
                .Include(p => p.ProductDetails)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null) return NotFound();
            var vm = new ProductUpdateVM
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Slug = product.Slug,
                Description = product.Description,
                Price = product.Price,
                Qty = product.Qty,
                Discount = product.Discount,
                Color = product.Color,
                Storage = product.Storage,
                Ram = product.Ram,
                Port = product.Port,
                Status = (byte)product.Status,
                ProductCategoryId = product.ProductCategoryId,
                Sku = product.ProductDetails?.FirstOrDefault()?.Sku ?? "",
                ExistingImages = product.ProductImages
                    .OrderByDescending(i => i.IsMain)
                    .ThenBy(i => i.ProductImageId)
                    .Select(i => new ExistingImageVM
                    {
                        ImageId = i.ProductImageId,
                        ImageUrl = i.ImageUrl!,
                        IsMain = i.IsMain
                    })
                    .ToList()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProductUpdateVM vm)
        {
            var categories = _db.ProductCategories
                .ToList();
            ViewBag.Categories = categories;
            if (!ModelState.IsValid)
            {

                // Trả lại view cùng lỗi validation
                return View(vm);
            }
            // 1) Lưu Product
            var product = new Product
            {
                ProductId = vm.ProductId,
                ProductName = vm.ProductName,
                Slug = vm.Slug,
                Description = vm.Description,
                Price = vm.Price,
                Qty = vm.Qty,
                Discount = vm.Discount,
                Color = vm.Color,
                Storage = vm.Storage,
                Ram = vm.Ram,
                Port = vm.Port,
                Status = (ActiveStatus)vm.Status,
                ProductCategoryId = vm.ProductCategoryId,
                UpdatedAt = DateTime.UtcNow
            };
            _db.Products.Update(product);
            await _db.SaveChangesAsync(); // có ProductId

            // 2) Lưu ProductDetail
            await _db.ProductDetails
                .Where(pd => pd.ProductId == product.ProductId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(pd => pd.Sku, vm.Sku)
                    .SetProperty(pd => pd.UpdatedAt, DateTime.UtcNow)
                );

            // 3) Lưu NHIỀU ảnh
            if (vm.ImageFiles != null && vm.ImageFiles.Count > 0)
            {
                var folder = Path.Combine(_env.WebRootPath, "images", "products");
                Directory.CreateDirectory(folder);

                // 1) XÓA TOÀN BỘ ẢNH CŨ (file vật lý + DB)
                var oldImages = await _db.ProductImages
                    .Where(pi => pi.ProductId == product.ProductId)
                    .ToListAsync();

                foreach (var img in oldImages)
                {
                    try
                    {
                        var oldPath = Path.Combine(folder, img.ImageUrl ?? "");
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }
                    catch
                    {
                        // TODO: log nếu cần, không chặn flow
                    }
                }
                _db.ProductImages.RemoveRange(oldImages);
                await _db.SaveChangesAsync(); // commit xóa trước khi thêm mới

                // Nếu người dùng không chọn index ảnh chính, dùng file đầu tiên (0)
                var mainIndex = (vm.MainImageIndex >= 0 && vm.MainImageIndex < vm.ImageFiles.Count)
                                ? vm.MainImageIndex
                                : 0;

                for (int i = 0; i < vm.ImageFiles.Count; i++)
                {
                    var file = vm.ImageFiles[i];
                    if (file == null || file.Length == 0) continue;

                    var ext = Path.GetExtension(file.FileName);
                    var fileName = $"Img_Product_{product.ProductId}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{i}{ext}";
                    var filePath = Path.Combine(folder, fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // LƯU DB CHỈ TÊN FILE
                    _db.ProductImages.Add(new ProductImage
                    {
                        ProductId = product.ProductId,
                        ImageUrl = fileName,          // <-- chỉ tên file
                        IsMain = (i == mainIndex),  // một ảnh chính
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Mac");
        }

    }
}



