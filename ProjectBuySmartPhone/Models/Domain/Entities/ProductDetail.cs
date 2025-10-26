using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ProjectBuySmartPhone.Models.Domain.Entities
{
    public class ProductDetail : BaseEntity
    {
        [Key]
        public int ProductDetailId { get; set; }

        [MaxLength(40)] 
        public string? Color { get; set; }
        [MaxLength(40)] 
        public string? Size { get; set; }
        [MaxLength(80)] 
        public string? Sku { get; set; }

        public int Qty { get; set; }

        [Column(TypeName = "decimal(18,2)")]

        public decimal Price { get; set; }

        // FK
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }

        // 1-n
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
