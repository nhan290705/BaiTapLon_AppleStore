using ProjectBuySmartPhone.Dtos.Common;
using ProjectBuySmartPhone.Models.Domain.Entities;

namespace ProjectBuySmartPhone.Dtos.Products
{
    public class SearchProductRequest : SearchPageRequest<Product>
    {

        public String? KeyWord { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinQuantity { get; set; }
        public int? MaxQuantity { get; set; }
    
        
        public void ValidateInput () {
            base.ValidateInput();

            if(KeyWord != null)
            {
                KeyWord = KeyWord.Trim().ToLower();
            }
        }
    }
}
