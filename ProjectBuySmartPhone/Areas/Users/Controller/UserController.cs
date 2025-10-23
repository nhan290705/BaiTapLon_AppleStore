using Microsoft.AspNetCore.Mvc;

namespace ProjectBuySmartPhone.Areas.Users.Controller
{
    public class UserController : ControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }

        private IActionResult View()
        {
            throw new NotImplementedException();
        }
    }
}
