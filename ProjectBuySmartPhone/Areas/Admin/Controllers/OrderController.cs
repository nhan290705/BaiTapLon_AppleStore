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
    public class OrderController : Controller
    { 
        private readonly IOrderRepository _orderRepository;
        private readonly IProductDetailRepository _productDetailRepository; 
        private readonly IOrderDetailRepository _orderDetailRepository;
        public OrderController(IOrderRepository orderRepository,IProductDetailRepository productDetailRepository, IOrderDetailRepository orderDetailRepository)
        {
            _orderRepository = orderRepository;
            _productDetailRepository = productDetailRepository;
            _orderDetailRepository = orderDetailRepository;
        }
        public IActionResult Index(string searchTerm, int? status, int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            // Lấy query từ repository
            var query = _orderRepository.GetAll();

            // Tìm kiếm theo searchTerm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchLower = searchTerm?.ToLower() ?? "";

                query = query.Where(o =>
                    o.OrderId.ToString().Contains(searchLower) ||
                    (o.RecipientName != null && o.RecipientName.ToLower().Contains(searchLower)) ||
                    (o.RecipientPhone != null && o.RecipientPhone.ToLower().Contains(searchLower))
                );

            }

            // Lọc theo status
            if (status.HasValue && status.Value > 0)
            {
                query = query.Where(o => o.StatusOrderId == status.Value);
            }

            // Sắp xếp
            query = query.OrderBy(x => x.OrderId);

            // Truyền giá trị vào ViewBag để giữ lại trong form
            ViewBag.SearchTerm = searchTerm;
            ViewBag.Status = status;

            // Phân trang
            var pagedList = query.ToPagedList(pageNumber, pageSize);

            return View(pagedList);
        }
        public IActionResult Details(string id)
        {
            var order = _orderRepository.getOrder(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }
        public IActionResult Edit(string id)
        {
            var order = _orderRepository.getOrder(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpPost]
        public IActionResult Edit(Order order)
        {
            if (ModelState.IsValid)
            {
                _orderRepository.update(order);
                return RedirectToAction("Index");
            }
            return View(order);
        }

        [Route("AddOrder")]
        [HttpGet]
        public IActionResult AddOrder()
        {
            // Load danh sách ProductDetail (bao gồm cả thông tin Product)
            var productDetails = _productDetailRepository.GetAll()
                .Include(pd => pd.Product) // Include Product để lấy tên
                .Where(pd => pd.Qty > 0) // Chỉ lấy sản phẩm còn hàng
                .Select(pd => new {
                    Value = pd.ProductDetailId,
                    Text = $"{pd.Product.ProductName} - {pd.Color} - {pd.Storage} - {pd.Ram}", // Hiển thị đầy đủ thông tin
                    Price = pd.Product.Price // Giá từ Product
                })
                .ToList();

            ViewBag.ProductDetails = productDetails;
            return View();
        }

        [Route("AddOrder")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrder(Order order, List<string> ProductDetailIds, List<int> Quantities, List<decimal> Prices)
        {
            if (ModelState.IsValid && ProductDetailIds != null && ProductDetailIds.Count > 0)
            {
                try
                {
                    // Tạo OrderId
                    order.OrderId = GenerateOrderId();
                    order.CreatedAt = DateTime.Now;
                    order.Status = 1; // Hoặc StatusOrderId tùy theo tên field trong DB

                    // Tính tổng tiền từ OrderDetails
                    decimal totalAmount = 0;
                    var orderDetails = new List<OrderDetail>();

                    for (int i = 0; i < ProductDetailIds.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(ProductDetailIds[i]))
                        {
                            // Kiểm tra số lượng tồn kho
                            var productDetail = _productDetailRepository.getProductDetail(ProductDetailIds[i]);
                            if (productDetail == null || productDetail.Product == null || productDetail.Product.Qty < Quantities[i])
                            {
                                ModelState.AddModelError("", $"Sản phẩm {productDetail?.Product?.ProductName ?? "Không xác định"} không đủ số lượng trong kho!");
                                LoadProductDetails();
                                return View(order);
                            }

                            // Tạo chi tiết đơn hàng
                            var detail = new OrderDetail
                            {
                                OrderDetailId = Guid.NewGuid().ToString(),
                                OrderId = order.OrderId,
                                ProductDetailId = ProductDetailIds[i],
                                Qty = Quantities[i],
                                UnitPrice = Prices[i],
                                LineTotal = Prices[i] * Quantities[i]
                            };

                            totalAmount += detail.LineTotal;
                            orderDetails.Add(detail);

                            // Trừ số lượng tồn kho (trên Product)
                            productDetail.Product.Qty -= Quantities[i];
                            _productDetailRepository.update(productDetail);
                        }
                    }

                    order.TotalPrice = totalAmount; // Dựa vào tên field trong DB của bạn

                    // Lưu Order
                    _orderRepository.add(order);

                    // Lưu OrderDetails
                    foreach (var detail in orderDetails)
                    {
                        _orderDetailRepository.add(detail);
                    }

                    TempData["SuccessMessage"] = $"Thêm đơn hàng {order.OrderId} thành công với tổng tiền {totalAmount:N0} VNĐ!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Có lỗi xảy ra: {ex.Message}");
                }
            }

            if (ProductDetailIds == null || ProductDetailIds.Count == 0)
            {
                ModelState.AddModelError("", "Vui lòng chọn ít nhất 1 sản phẩm!");
            }

            LoadProductDetails();
            return View(order);
        }

        private void LoadProductDetails()
        {
            var productDetails = _productDetailRepository.GetAll()
                .Include(pd => pd.Product)
                .Where(pd => pd.Qty > 0)
                .Select(pd => new {
                    Value = pd.ProductDetailId,
                    Text = $"{pd.Product.ProductName} - {pd.Color} - {pd.Storage} - {pd.Ram}",
                    Price = pd.Product.Price
                })
                .ToList();

            ViewBag.ProductDetails = productDetails;
        }

        private int GenerateOrderId()
        {
            // Lấy số lượng order từ repository
            var count = _orderRepository.GetAll().Count(o => o.CreatedAt.Date == DateTime.Today) + 1;
            
            return count;
        }
    }
}
