using System.ComponentModel.DataAnnotations;

namespace ProjectBuySmartPhone.Models.Domain.Entities
{
    public class ProductCategory : BaseEntity
    {
        [Key]
        public int ProductCategoryId { get; set; }

        [Required, MaxLength(160)]

        public string CategoryName { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(160)]
        public string? Slug { get; set; }

        // 1-n
        public virtual ICollection<Product>? Products { get; set; }
    }
}
