using System.ComponentModel.DataAnnotations;

namespace ProjectBuySmartPhone.Models.Domain.Entities
{
    public abstract class BaseEntity
    {
        [Required] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
