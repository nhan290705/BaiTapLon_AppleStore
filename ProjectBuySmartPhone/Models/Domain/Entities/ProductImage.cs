using System.ComponentModel.DataAnnotations;

namespace ProjectBuySmartPhone.Models.Domain.Entities
{
    public class ProductImage : BaseEntity
    {
        [Key]
        public int ProductImageId { get; set; }

        [Required, MaxLength(300)]
        public string ImageUrl { get; set; }

        public bool IsMain { get; set; } = false;

        // FK
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
    }
}
