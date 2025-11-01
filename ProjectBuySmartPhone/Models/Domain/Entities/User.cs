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
        public string FirstName { get; set; }
        [Required, MaxLength(120)]
        public string LastName { get; set; }
        public string? Address;

        [Required, MaxLength(160)] 
        public string Email { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(100)]
        public string Username { get; set; }


        [Required, MaxLength(100)]
        public string Password { get; set; }
        public string? Role { get; set; } = RoleName.USER.ToString();
 

        public ActiveStatus IsActive { get; set; } = ActiveStatus.Active;

        // 1-n
        public virtual ICollection<Blog>? Blogs { get; set; }
        public virtual ICollection<BlogComment>? BlogComments { get; set; }
        public virtual ICollection<ProductComment>? ProductComments { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
    }
}
