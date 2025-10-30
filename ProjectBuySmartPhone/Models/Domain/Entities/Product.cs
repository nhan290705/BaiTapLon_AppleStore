using ProjectBuySmartPhone.Models.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectBuySmartPhone.Models.Domain.Entities
{
    public class Product : BaseEntity
    {
        [Key]
        public int ProductId { get; set; }

        [Required, MaxLength(200)]
        public string? ProductName { get; set; }

        [MaxLength(200)]
        public string? Slug { get; set; }

        public string? Description { get; set; }

        //[Precision(18, 2)]
        public decimal Price { get; set; }

        public int Qty { get; set; }

        //[Precision(5, 2)]
        public decimal Discount { get; set; }

        public string? Color { get; set; }

        public decimal? Storage {  get; set; }

        public decimal? Ram { get; set; }
        public string? Port {  get; set; }


        public ActiveStatus Status { get; set; } = ActiveStatus.Active;

        // FK -> Category
        public int ProductCategoryId { get; set; }
        public virtual ProductCategory? ProductCategory { get; set; }

        // 1-n
        public virtual ICollection<ProductImage>? ProductImages { get; set; }
        public virtual ICollection<ProductDetail>? ProductDetails { get; set; }
        public virtual ICollection<ProductComment>? ProductComments { get; set; }
    }
}
