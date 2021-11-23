using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using qwerty.Data;
using qwerty.Models;

namespace qwerty.Controllers
{
    public class UserSessionController : Controller
    {
        private readonly QwertyContext _context;
        public IConfiguration Configuration { get; }
        public string connString;

        public UserSessionController(QwertyContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
            connString = Configuration.GetConnectionString("Default");
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
            List<Owner> from_store = new List<Owner>();
            using (var conn = new MySqlConnection(connString))
            {

                var cmd = conn.CreateCommand();
                await conn.OpenAsync();
                cmd.CommandText = "s_FindUser";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@i_visible", 1);
                cmd.Parameters.AddWithValue("@st_own", owner.own);
                cmd.Parameters.AddWithValue("@st_pswd", owner.pswd);

                var reader = cmd.ExecuteReader();
                if (reader.HasRows == true)
                {
                    foreach (var item in reader)
                    {

                        var row = new Owner
                        {
                            Id = reader.GetInt16("Id"),
                            own = reader.GetString("own"),
                            DepartmentId = reader.GetInt16("DepartmentId"),
                            PermissionId = reader.GetInt16("PermissionId"),
                            visible = reader.GetInt16("visible"),
                            pswd = reader.GetString("pswd")
                        };
                        from_store.Add(row);

                    }


                }

            }
          
            // var findUser = _context.Owner.Where(s => s.own == owner.own).Where(s =>s.visible==1).Where(s => s.pswd == owner.pswd);

            if (from_store.Count() > 0)
            {
                var user = from_store.FirstOrDefault();
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