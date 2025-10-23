using System.ComponentModel.DataAnnotations;

namespace ProjectBuySmartPhone.Models.Domain.Entities
{
    public class BlogComment : BaseEntity
    {
        [Key]
        public int BlogCommentId { get; set; }

        [Required, MaxLength(1000)]
        public string Message { get; set; }

        // FK
        public int BlogId { get; set; }
        public virtual Blog? Blog { get; set; }

        public int UserId { get; set; }
        public virtual User? User { get; set; }
    }
}
