using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using qwerty.Data;
using qwerty.Models;

namespace qwerty.Controllers
{
    public class PermissionController : Controller
    {
        private readonly QwertyContext _context;

        public PermissionController(QwertyContext context)
        {
            _context = context;
        }

        // GET: Permission
        public async Task<IActionResult> Index()
        {
      
            return View(await _context.Permission.ToListAsync());
        }

        // GET: Permission/Create
        public IActionResult Create()
        {
            var Permission_List = _context.Permission.ToList();
            List<SelectListItem> Select_List = new List<SelectListItem>(); 
            return View();
        }

        // POST: Permission/Create
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,permission")] Permission per)
        {
            if (ModelState.IsValid)
            {
                _context.Add(per);
                await _context.SaveChangesAsync();
                // return RedirectToAction(nameof(Index));    
                return RedirectToAction("Create", "UserPer");

            }
            return View(per);
        }

        // GET: Permission/Edit/5


        // POST: Permission/Edit/5
         

        // GET: Permission/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var permission = await _context.Permission
                .FirstOrDefaultAsync(m => m.Id == id);
            if (permission == null)
            {
                return NotFound();
            }
            return View(permission);
        }

        // POST: Permission/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var permission = await _context.Permission.FindAsync(id);
            _context.Permission.Remove(permission);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PermissionExists(int id)
        {
            return _context.Permission.Any(e => e.Id == id);
        }
 
    }
}
