
namespace ProjectBuySmartPhone.Models.Domain.Session
{
    public class ShoppingCart
    {
        public List<CartItem> Items { get; set; } = new();
        public decimal TotalAmount => Items.Sum(x => x.Total);
    }
}
