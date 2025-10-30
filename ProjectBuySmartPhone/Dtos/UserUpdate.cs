namespace ProjectBuySmartPhone.Dtos
{
    public class UserUpdate
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;

        // password
        public string? currentPassword { get; set; }
        public string? newPassword { get; set; }
        public string? confirmNewPassword { get; set; }
    }
}
