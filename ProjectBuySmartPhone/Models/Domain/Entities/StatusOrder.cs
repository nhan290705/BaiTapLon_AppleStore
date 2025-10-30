using ProjectBuySmartPhone.Models.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBuySmartPhone.Models.Domain.Entities
{
    public class StatusOrder
    {
        [Key]
        public int StatusOrderId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; } = null!; // "Pending", "Paid", "Shipped"...

        [MaxLength(200)]
        public string? Description { get; set; }

        // Quan he 1-1 voi Order
        public virtual ICollection<Order>? Orders { get; set; }
    }
}
