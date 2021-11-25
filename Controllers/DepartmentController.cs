using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using qwerty.Data;
using qwerty.Models;

namespace qwerty.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly QwertyContext _context;
        public IConfiguration Configuration { get; }
        public string connString;

        public DepartmentController(QwertyContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
            connString = Configuration.GetConnectionString("Default");
        }


        public async Task<IActionResult> Index()
        {
            List<Department> Department_store = new List<Department>();
            using (var conn = new MySqlConnection(connString))
            {
                var cmd = conn.CreateCommand();
                await conn.OpenAsync();
                cmd.CommandText = "s_GetDepartment";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@i_visible", 1);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows == true)
                {
                    foreach(var item in reader)
                    {

                        var row = new Department
                        {
                            Id = reader.GetInt16("Id"),
                            department = reader.GetString("department"),
                      

                        };
                        Department_store.Add(row);
                    }
                }
            }
            return View(Department_store);
        }

        public IActionResult Create()
        {     
            return View();
            
        }

        // POST: Permission/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Create([Bind("Id,department")] Department depart)
        public async Task<IActionResult> Create(Department depart)
        {
            List<Department> from_store = new List<Department>();
            if (ModelState.IsValid)
            {

                using (var conn = new MySqlConnection(connString))
                {
                    var cmd = conn.CreateCommand();
                    await conn.OpenAsync();
                    cmd.CommandText = "s_CreateDepartment";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@st_department", depart.department);
                    cmd.Parameters.AddWithValue("@i_visible", 1);
                    var reader = cmd.ExecuteNonQuery();

                }
                return RedirectToAction("Create", "Owner");
            }
            return View();
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var Department = await _context.Department.Where(s => s.visible == 1).FirstOrDefaultAsync(m => m.Id == id);
            if (Department == null)
            {
                return NotFound();
            }
            return View(Department);
        }

        // POST: Permission/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var Department = await _context.Department.FindAsync(id);
            Department.visible = 0;
            _context.Department.Update(Department);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}