using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBuySmartPhone.Models.Domain.Entities
{
    public class OrderDetail : BaseEntity
    {
        [Key]
        [MaxLength(200)] public int? OrderDetailId { get; set; }

        public int Qty { get; set; }

        [Column(TypeName = "decimal(18,2)")]

        public decimal UnitPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]

        public decimal LineTotal { get; set; }

        // FKs
        public int OrderId { get; set; }
        public virtual Order? Order { get; set; }

        public virtual ICollection<ProductDetail>? ProductDetails { get; set; }
    }
}
