using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ProjectBuySmartPhone.Models.Domain.Entities
{
    public class ProductDetail : BaseEntity
    {
        [Key]
        public int ProductDetailId { get; set; }

        [MaxLength(40)] 
        public string? Sku { get; set; }


        // FK
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }

        // 1-n
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
