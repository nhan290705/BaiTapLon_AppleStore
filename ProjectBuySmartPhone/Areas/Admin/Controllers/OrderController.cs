using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBuySmartPhone.Models;
using ProjectBuySmartPhone.Models.Domain.Entities;
using ProjectBuySmartPhone.Models.Infrastructure;
using ProjectBuySmartPhone.Responsitory;
using X.PagedList;

namespace ProjectBuySmartPhone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    [Authorize(Roles = "ADMIN")]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductDetailRepository _productDetailRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IStatusResponsitory _statusResponsitory;
        public OrderController(IOrderRepository orderRepository, IProductDetailRepository productDetailRepository, 
                        IOrderDetailRepository orderDetailRepository,IStatusResponsitory statusResponsitory)
        {
            _orderRepository = orderRepository;
            _productDetailRepository = productDetailRepository;
            _orderDetailRepository = orderDetailRepository;
            _statusResponsitory = statusResponsitory;
        }

        public IActionResult Index(string searchTerm, int? status, int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var query = _orderRepository.GetAll();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchLower = searchTerm.ToLower();
                query = query.Where(o =>
                    o.OrderId.ToString().Contains(searchLower) ||
                    (o.RecipientName != null && o.RecipientName.ToLower().Contains(searchLower)) ||
                    (o.RecipientPhone != null && o.RecipientPhone.ToLower().Contains(searchLower))
                );
            }

            if (status.HasValue && status.Value > 0)
            {
                query = query.Where(o => o.StatusOrderId == status.Value);
            }

            query = query.OrderByDescending(x => x.CreatedAt);

            ViewBag.SearchTerm = searchTerm;
            ViewBag.Status = status;

            var pagedList = query.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        public IActionResult Details(int id)
        {
            var order = _orderRepository.GetAll()
                .Include(o => o.StatusOrder)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProductDetails)
                        .ThenInclude(pd => pd.Product)
                            .ThenInclude(p => p.ProductCategory)
                .FirstOrDefault(o => o.OrderId  == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var order = _orderRepository.getOrder(id);
            if (order == null)
            {
                return NotFound();
            }

            // Đảm bảo load danh sách trạng thái
            var statusOrders = _statusResponsitory.GetAll().ToList();

            if (statusOrders == null || !statusOrders.Any())
            {
                // Log lỗi nếu không có dữ liệu trạng thái
                ModelState.AddModelError("", "Không thể tải danh sách trạng thái đơn hàng.");
                statusOrders = new List<StatusOrder>();
            }

            ViewBag.StatusOrders = statusOrders;

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Order order)
        {
            // Loại bỏ các trường không cần validate
            ModelState.Remove("User");
            ModelState.Remove("StatusOrder");
            ModelState.Remove("OrderDetails");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                try
                {
                    var orderToUpdate = _orderRepository.getOrder(order.OrderId);
                    if (orderToUpdate == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật các trường từ form
                    orderToUpdate.StatusOrderId = order.StatusOrderId;
                    orderToUpdate.RecipientName = order.RecipientName;
                    orderToUpdate.RecipientPhone = order.RecipientPhone;
                    orderToUpdate.ShippingAddress = order.ShippingAddress;
                    orderToUpdate.City = order.City;
                    orderToUpdate.District = order.District;
                    orderToUpdate.PostalCode = order.PostalCode;
                    orderToUpdate.PaymentMethod = order.PaymentMethod;
                    orderToUpdate.ShippingMethod = order.ShippingMethod;
                    orderToUpdate.Note = order.Note;

                    _orderRepository.update(orderToUpdate);

                    TempData["SuccessMessage"] = $"Cập nhật đơn hàng #{order.OrderId} thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Không thể lưu thay đổi. " + ex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
                }
            }
            else
            {
                // Debug ModelState errors
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state.Errors.Count > 0)
                    {
                        foreach (var error in state.Errors)
                        {
                            System.Diagnostics.Debug.WriteLine($"❌ [{key}]: {error.ErrorMessage}");
                        }
                    }
                }
            }

            ViewBag.StatusOrders = _statusResponsitory.GetAll().ToList();
            return View(order);
        }

        [Route("AddOrder")]
        [HttpGet]
        public IActionResult AddOrder()
        {
            LoadAvailableProductDetails();

            var model = new Order();

            return View(model);
        }

        [Route("AddOrder")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrder([Bind("UserId", "RecipientName", "RecipientPhone", "ShippingAddress", "City", "District", "PostalCode", "PaymentMethod", "ShippingMethod", "Note")] Order order, List<string> ProductDetailIds)
        {
            
            if (ProductDetailIds == null || ProductDetailIds.Count == 0 || ProductDetailIds.All(string.IsNullOrEmpty))
            {
                ModelState.AddModelError("", "Vui lòng chọn ít nhất 1 sản phẩm!");
                LoadAvailableProductDetails();
                return View(order);
            }
             
            if (!order.UserId.HasValue || order.UserId.Value == 0)
            {
                order.UserId = null;
                ModelState.Remove("UserId");  
            }
             
            if (!ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine("❌❌❌ ModelState KHÔNG HỢP LỆ:");
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state.Errors.Count > 0)
                    {
                        foreach (var error in state.Errors)
                        {
                            System.Diagnostics.Debug.WriteLine($"  ❌ [{key}]: {error.ErrorMessage}");
                        }
                    }
                }
                LoadAvailableProductDetails();
                return View(order);
            }

            try
            { 
                order.CreatedAt = DateTime.Now;
                order.StatusOrderId = 1;
                order.TotalPrice = 0;

                _orderRepository.add(order);

                System.Diagnostics.Debug.WriteLine($"✅ Order {order.OrderId} đã được thêm");

                var validProductDetailIds = ProductDetailIds
                    .Where(id => !string.IsNullOrEmpty(id))
                    .Select(id => int.Parse(id))
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"✅ Có {validProductDetailIds.Count} sản phẩm");
 
                var orderDetail = new OrderDetail
                { 
                    OrderId = order.OrderId,  
                    Qty = validProductDetailIds.Count,
                    UnitPrice = 0,
                    LineTotal = 0,
                    CreatedAt = DateTime.Now
                };

                _orderDetailRepository.add(orderDetail);
                 
                System.Diagnostics.Debug.WriteLine($"✅ OrderDetail {orderDetail.OrderDetailId} đã được thêm");
                 
                decimal totalAmount = 0;

                foreach (var productDetailId in validProductDetailIds)
                {
                    var productDetail = _productDetailRepository.GetAll()
                        .Include(pd => pd.Product)
                        .FirstOrDefault(pd => pd.ProductDetailId == productDetailId);

                    if (productDetail == null)
                    {
                        throw new Exception($"Không tìm thấy sản phẩm với ID: {productDetailId}");
                    }

                    if (productDetail.OrderDetailId != null)
                    {
                        throw new Exception($"Sản phẩm '{productDetail.Product?.ProductName}' đã thuộc đơn hàng khác!");
                    }
                     
                    productDetail.OrderDetailId = orderDetail.OrderDetailId;
                    _productDetailRepository.update(productDetail);

                    if (productDetail.Product != null)
                    {
                        totalAmount += productDetail.Product.Price;
                        System.Diagnostics.Debug.WriteLine($"✅ Thêm sản phẩm {productDetail.Product.ProductName}: {productDetail.Product.Price:N0} VNĐ");
                    }
                }
                 
                orderDetail.UnitPrice = totalAmount / orderDetail.Qty;
                orderDetail.LineTotal = totalAmount;
                _orderDetailRepository.update(orderDetail);

                System.Diagnostics.Debug.WriteLine($"✅ OrderDetail: UnitPrice={orderDetail.UnitPrice:N0}, LineTotal={orderDetail.LineTotal:N0}");

                order.TotalPrice = totalAmount;
                _orderRepository.update(order);

                System.Diagnostics.Debug.WriteLine($"✅ Order TotalPrice: {order.TotalPrice:N0} VNĐ");

                TempData["SuccessMessage"] = $"Thêm đơn hàng #{order.OrderId} thành công với tổng tiền {totalAmount:N0} VNĐ!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                Exception ex = dbEx;
                var errorMessages = new List<string>();

                while (ex != null)
                {
                    errorMessages.Add(ex.Message);
                    System.Diagnostics.Debug.WriteLine($"🔴 {ex.Message}");
                    ex = ex.InnerException;
                }

                var fullError = string.Join(" → ", errorMessages);
                ModelState.AddModelError("", fullError);
                LoadAvailableProductDetails();
                return View(order);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ {ex.Message}");
                ModelState.AddModelError("", ex.Message);
                LoadAvailableProductDetails();
                return View(order);
            }
        }

       

        private void LoadAvailableProductDetails()
        {
            // Chỉ lấy các ProductDetail chưa thuộc OrderDetail nào (OrderDetailId == null)
            var productDetails = _productDetailRepository.GetAll()
                .Include(pd => pd.Product)
                    .ThenInclude(p => p.ProductCategory) // Include Category để lấy tên danh mục
                .Where(pd => pd.OrderDetailId == null && pd.Product != null && pd.Product.Qty > 0)
                .ToList() // Lấy dữ liệu về client trước
                .Select(pd => new
                {
                    Value = pd.ProductDetailId,
                    Text = $"{pd.Product.ProductName} - {pd.Product.Color} - {pd.Product.Storage} - {pd.Product.Ram}",
                    Price = pd.Product.Price,
                    CategoryName = pd.Product.ProductCategory != null ? pd.Product.ProductCategory.CategoryName : "Khác"
                })
                .OrderBy(pd => pd.CategoryName)
                .ThenBy(pd => pd.Text)
                .ToList();

            ViewBag.ProductDetails = productDetails;
        }

    }
}