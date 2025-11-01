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
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Qty { get; set; }

        //[Precision(5, 2)]

        public decimal Discount { get; set; }

        [Required]
        public string Color { get; set; }
        [Required]
        public string Storage {  get; set; }
        [Required]
        public string Ram { get; set; }
        [Required]
        public string Port {  get; set; }
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
