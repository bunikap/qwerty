using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using qwerty.Data;
using qwerty.Models;

namespace qwerty.Controllers
{
    public class UserSessionController : Controller
    {
        private readonly QwertyContext _context;

        public UserSessionController(QwertyContext context)
        {
            _context = context;
        }

        // GET: Permission
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("Id,own,DepartmentId,PermissionId,pswd")] Owner owner)
        {

            var findUser = _context.Owner.Where(s => s.own == owner.own).Where(s => s.pswd == owner.pswd);

            if (findUser.Count() > 0)
            {
                var user = findUser.FirstOrDefault();
                ViewBag.own = user;
                HttpContext.Session.SetString("userName", user.own);
                HttpContext.Session.SetInt32("userId", user.Id);
                var test = HttpContext.Session.GetString("userName");
                return RedirectToAction("Index", "Home");

            }


            return View();



        }


        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();

            var test = HttpContext.Session.GetString("userName");
            return RedirectToAction("Index", "UserSession");


        }


        [HttpGet]
        public JsonResult Check()
        {
            if (HttpContext.Session.GetString("userName") == null)
            {
                var pass = false;
                return Json(new { result = pass });

            }
            else
            {
                var pass = true;
                return Json(new { result = pass });
            }

        }


    }
}