using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ProjectBuySmartPhone.Areas.Admin.Models.ViewModels
{
    // ProductId là IDENTITY tự tăng => không đưa vào VM
    public class ProductCreateVM
    {
        // Product
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        public string ProductName { get; set; } = string.Empty;
        public string? Slug { get; set; }
        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải ít nhất là 1")]
        public int Qty { get; set; }

        [Range(0, 100, ErrorMessage = "Giảm giá phải từ 0 đến 100%")]
        public decimal Discount { get; set; }
        public string? Color { get; set; }

        [Required(ErrorMessage = "Bộ nhớ (Storage) là bắt buộc")]
        public string Storage { get; set; }
        [Required(ErrorMessage = "RAM là bắt buộc")]
        public string Ram { get; set; }

        [Required(ErrorMessage = "Cổng kết nối là bắt buộc")]
        public string Port { get; set; }
        public byte Status { get; set; } = 1;

        [Required(ErrorMessage = "Phải chọn danh mục sản phẩm")]
        [Range(1, 5, ErrorMessage = "Danh mục sản phẩm chỉ hợp lệ từ 1 đến 5")]
        public int ProductCategoryId { get; set; }   // nếu bạn muốn chỉ 1..5, view sẽ render radio 5 lựa chọn

        // ProductDetail
        [Required(ErrorMessage = "Mã SKU là bắt buộc")]
        public string Sku { get; set; } = string.Empty;

        // ProductImage (chỉ 1 ảnh)
        //public IFormFile? ImageFile { get; set; }
        //[Required(ErrorMessage = "Phải chọn ít nhất một hình ảnh")]
        public List<IFormFile>? ImageFiles { get; set; }

        // (tuỳ chọn) index ảnh chính người dùng chọn trên form; -1 = tự lấy file đầu tiên làm ảnh chính
        public int MainImageIndex { get; set; } = -1;
    }
}
