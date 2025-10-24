using Microsoft.AspNetCore.Mvc;
using ProjectBuySmartPhone.Dtos;
using ProjectBuySmartPhone.Models.Infrastructure;

namespace ProjectBuySmartPhone.Areas.Identity.Controller
{
   
    public class RegisterController : ControllerBase
    {
        private readonly ILogger<RegisterController> _logger;
        private readonly MyDbContext _context;

        public RegisterController(ILogger<RegisterController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        private IActionResult View()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public IActionResult Register(UserRegister userRegister)
        {

                       return View();
        }
    }
}
