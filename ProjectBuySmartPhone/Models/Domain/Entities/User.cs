using ProjectBuySmartPhone.Models.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace ProjectBuySmartPhone.Models.Domain.Entities
{
    public class User : BaseEntity
    {
        [Key]
        public int UserId { get; set; }

        public string UserName { get; set; }


        [Required, MaxLength(120)]
        public string? FullName { get; set; }

        [Required, MaxLength(160)]
<<<<<<< HEAD
        public string Email { get; set; }

        [Required, MaxLength(200)]
        public string Password { get; set; }
=======
        public string? Email { get; set; }
>>>>>>> origin/han

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }
        
        [MaxLength(100)]
        public string? Username { get; set; }

<<<<<<< HEAD

        [MaxLength(50)]
        public string Role { get; set; } = "User";

        public int Level { get; set; } = 1;

        [MaxLength(500)]
        public string? Description { get; set; }
=======
        [Required, MaxLength(100)]
        public string? Password { get; set; }
        public string? Role { get; set; } = RoleName.USER.ToString();
>>>>>>> origin/han

        public ActiveStatus IsActive { get; set; } = ActiveStatus.Active;

        // 1-n
        public virtual ICollection<Blog>? Blogs { get; set; }
        public virtual ICollection<BlogComment>? BlogComments { get; set; }
        public virtual ICollection<ProductComment>? ProductComments { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
    }
}
