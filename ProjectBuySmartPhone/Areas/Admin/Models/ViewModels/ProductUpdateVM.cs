using Microsoft.AspNetCore.Http;

namespace ProjectBuySmartPhone.Areas.Admin.Models.ViewModels
{
    public class ProductUpdateVM : ProductCreateVM
    {
        public int ProductId { get; set; }
        public List<ExistingImageVM> ExistingImages { get; set; } = new();
    }
}
