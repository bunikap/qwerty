using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using qwerty.Data;
using qwerty.Models;

namespace qwerty.Controllers
{
    public class OwnerController : Controller
    {
        private readonly QwertyContext _context;

        public OwnerController(QwertyContext context)
        {
            _context = context;
        }

        // GET: Owner
        public async Task<IActionResult> Index()
        {
            var qwertyContext = _context.Owner.Include(o => o.Permissionn).Include(o => o.departmentt);

            return View(await qwertyContext.ToListAsync());


        }

        // GET: Owner/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owner.Include(o => o.Permissionn).Include(o => o.departmentt).FirstOrDefaultAsync(m => m.Id == id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        // GET: Owner/Create
        public IActionResult Create()
        {
            ViewData["PermissionId"] = new SelectList(_context.Permission.Where(s => s.visible == 1), "Id", "permission");
            ViewData["DepartmentId"] = new SelectList(_context.Department.Where(s => s.visible == 1), "Id", "department");

            return View();
        }

        // POST: Owner/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,own,DepartmentId,PermissionId")] Owner owner)
        {
            if (ModelState.IsValid)
            {
                owner.visible = 1;
                _context.Add(owner);
                await _context.SaveChangesAsync();
                var Userper = new UserPer { OwnerId = owner.Id, PermissionsId = owner.PermissionId, visible = 1 };
                _context.Add(Userper);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PermissionId"] = new SelectList(_context.Permission.Where(s => s.visible == 1), "Id", "permission", owner.PermissionId);
            ViewData["DepartmentId"] = new SelectList(_context.Department.Where(s => s.visible == 1), "Id", "department", owner.DepartmentId);

            return View(owner);



        }

        // GET: Owner/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var owner = await _context.Owner.FindAsync(id);
            if (owner == null)
            {
                return NotFound();
            }
            ViewData["PermissionId"] = new SelectList(_context.Permission.Where(s => s.visible == 1), "Id", "permission", owner.PermissionId);
            ViewData["DepartmentId"] = new SelectList(_context.Department.Where(s => s.visible == 1), "Id", "department", owner.DepartmentId);
            return View(owner);
        }

        // POST: Owner/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,own,DepartmentId,PermissionId")] Owner owner)
        {
            if (id != owner.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    owner.visible=1;
                    _context.Update(owner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OwnerExists(owner.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PermissionId"] = new SelectList(_context.Permission.Where(s=>s.visible==1), "Id", "permission", owner.PermissionId);
            ViewData["DepartmentId"] = new SelectList(_context.Department.Where(s => s.visible == 1), "Id", "department", owner.DepartmentId);

            return View(owner);
        }

        // GET: Owner/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owner.Include(o => o.Permissionn).FirstOrDefaultAsync(m => m.Id == id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        // POST: Owner/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var owner = await _context.Owner.FindAsync(id);
            _context.Owner.Remove(owner);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OwnerExists(int id)
        {
            return _context.Owner.Any(e => e.Id == id);
        }
    }
}
