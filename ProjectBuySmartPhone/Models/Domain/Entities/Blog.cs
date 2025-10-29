using ProjectBuySmartPhone.Models.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBuySmartPhone.Models.Domain.Entities
{
    public class Blog : BaseEntity
    {
        [Key]
        public int BlogId { get; set; }

        [Required, MaxLength(220)]
        public string Title { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string? Content { get; set; }

        public BlogStatus Status { get; set; } = BlogStatus.Draft;

        // FK
        public int UserId { get; set; }
        public virtual User? User { get; set; }

        // 1-n
        public virtual ICollection<BlogComment>? BlogComments { get; set; }
    }
}
