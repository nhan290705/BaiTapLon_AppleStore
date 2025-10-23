using ProjectBuySmartPhone.Models.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectBuySmartPhone.Models.Domain.Entities
{
    public class Order : BaseEntity
    {
        [Key]
        public int OrderId { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cod;
        public ShippingMethod ShippingMethod { get; set; } = ShippingMethod.Standard;

        [MaxLength(200)] public string? RecipientName { get; set; }
        [MaxLength(20)] public string? RecipientPhone { get; set; }
        [MaxLength(300)] public string? ShippingAddress { get; set; }
        [MaxLength(100)] public string? City { get; set; }
        [MaxLength(100)] public string? District { get; set; }
        [MaxLength(20)] public string? PostalCode { get; set; }

        // FK
        public int UserId { get; set; }
        public virtual User? User { get; set; }

        // 1-n
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
