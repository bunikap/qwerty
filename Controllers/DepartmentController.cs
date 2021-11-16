using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using qwerty.Data;
using qwerty.Models;

namespace qwerty.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly QwertyContext _context;
        public DepartmentController(QwertyContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {

            return View();
        }
        public IActionResult Create()
        {
            var Department_List = _context.Department.Where(s => s.visible == 1).ToList();
            List<SelectListItem> Select_List = new List<SelectListItem>();
            return View();
        }

        // POST: Permission/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,department")] Department depart)
        {
            if (ModelState.IsValid)
            {
                depart.visible = 1;
                _context.Add(depart);
                await _context.SaveChangesAsync();
                // return RedirectToAction(nameof(Index));    
                return RedirectToAction("Create", "Owner");

            }
            return View(depart);
        }

    }
}