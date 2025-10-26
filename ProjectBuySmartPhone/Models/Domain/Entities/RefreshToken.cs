using System.ComponentModel.DataAnnotations;

namespace ProjectBuySmartPhone.Models.Domain.Entities
{
    public class RefreshToken
    {
        [Key]
        public int id { get; set; }
        public string? TokenName { get; set; }

        public int UserId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool isActive { get; set; } = true;
        public virtual User? User { get; set; }
    }
}
