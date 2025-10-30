using ProjectBuySmartPhone.Models;
using Microsoft.AspNetCore.Mvc;
using ProjectBuySmartPhone.Responsitory;
namespace ProjectBuySmartPhone.ViewComponents
{
    public class StatusOrderMenuViewComponent: ViewComponent
    {
        private readonly IStatusResponsitory _statusResponsitory;
        public StatusOrderMenuViewComponent(IStatusResponsitory statusRespon)
        {
            _statusResponsitory = statusRespon;
        }
        public IViewComponentResult Invoke()
        {
            var statusRespon = _statusResponsitory.GetAll().OrderBy(x => x.StatusOrderId);
            return View(statusRespon);
        }
    }
}
