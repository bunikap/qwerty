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


         public async Task<IActionResult> Index()
        {
            return View(await _context.Department.Where(s=>s.visible==1).ToListAsync());
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


         public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var Department = await _context.Department.Where(s=>s.visible==1).FirstOrDefaultAsync(m => m.Id == id);
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
            Department.visible=0;
            _context.Department.Update(Department);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}