using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace qwerty.Controllers
{
    public class newController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}