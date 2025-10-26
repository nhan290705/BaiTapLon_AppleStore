using ProjectBuySmartPhone.Models.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace ProjectBuySmartPhone.Models.Domain.Entities
{
    public class User : BaseEntity
    {
        [Key]
        public int UserId { get; set; }

        [Required, MaxLength(120)]
        public string FullName { get; set; }

        [Required, MaxLength(160)]
        public string Email { get; set; }

        [Required, MaxLength(200)]
        public string PasswordHash { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(300)]
        public string? Avatar { get; set; }

        [MaxLength(50)]
        public string Role { get; set; } = "User";

        public int Level { get; set; } = 1;

        [MaxLength(500)]
        public string? Description { get; set; }

        public ActiveStatus IsActive { get; set; } = ActiveStatus.Active;

        // 1-n
        public virtual ICollection<Blog>? Blogs { get; set; }
        public virtual ICollection<BlogComment>? BlogComments { get; set; }
        public virtual ICollection<ProductComment>? ProductComments { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
    }
}
