using ProjectBuySmartPhone.Models.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProjectBuySmartPhone.Dtos
{
    public class UserRegister
    {
        [Required]
        public string? FullName { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }
        [Required]
        [RegularExpression(@"^(03|09)\d{8}$", ErrorMessage = "Invalid phone format")]
        public string? PhoneNumber { get; set; }
        [Required]
        [MinLength(4, ErrorMessage = "Username must be at least 4 characters long")]
        public string? Username { get; set; }
        [Required]
        [MinLength(4, ErrorMessage = "Password must be at least 4 characters long")]
        public string? Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }
        public User ToUser()
        {
            return new User
            {
                FullName = this.FullName,
                Email = this.Email,
                PhoneNumber = this.PhoneNumber,
                Username = this.Username,
                UpdatedAt = DateTime.Now,
            };

        }
    }
}
