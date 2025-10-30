namespace ProjectBuySmartPhone.Areas.Admin.Models.ViewModels
{
    public class ExistingImageVM
    {
        public int ImageId { get; set; }
        public string ImageUrl { get; set; } = "";
        public bool IsMain { get; set; }
    }
}
