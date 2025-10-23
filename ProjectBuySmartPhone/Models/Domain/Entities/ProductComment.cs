using System.ComponentModel.DataAnnotations;

namespace ProjectBuySmartPhone.Models.Domain.Entities
{
    public class ProductComment : BaseEntity
    {
        [Key]
        public int ProductCommentId { get; set; }

        [Required, MaxLength(1000)]
        public string Message { get; set; }

        public int Rating { get; set; }  // 1..5

        // FK
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }

        public int UserId { get; set; }
        public virtual User? User { get; set; }
    }
}
