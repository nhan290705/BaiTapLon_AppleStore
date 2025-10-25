using Microsoft.AspNetCore.Mvc;
using ProjectBuySmartPhone.Models.Infrastructure;

namespace ProjectBuySmartPhone.Areas.Users.Controller
{
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly MyDbContext _context;
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
