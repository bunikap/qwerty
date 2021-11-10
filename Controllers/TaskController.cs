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
    public class TaskController : Controller
    {
        private readonly QwertyContext _context;

        public TaskController(QwertyContext context)
        {
            _context = context;
        }

        // GET: Task
        public async Task<IActionResult> Index()
        {
            var qwertyContext = _context.Tasks.Include(t => t.Approve).Include(t => t.Owners).Include(t => t.stat);
            return View(await qwertyContext.ToListAsync());
        }

        // GET: Task/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks
                .Include(t => t.Approve)
                .Include(t => t.Owners)
                .Include(t => t.stat)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tasks == null)
            {
                return NotFound();
            }

            return View(tasks);
        }

        // GET: Task/Create
        public IActionResult Create()
        {
            var name_list = from m in _context.UserPer
                            join n in _context.Owner on m.OwnerId equals n.Id
                            select new { Id = m.OwnerId, name = n.own, p = m.PermissionsId};

            ViewData["ApproveId"] = new SelectList(name_list.Where(s => s.p == 1), "Id", "name");
            ViewData["OwnersId"] = new SelectList(name_list.Where(s => s.p == 2), "Id", "name");
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "status");
            return View();
        }

        // POST: Task/Create
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Detail,SDate,DDate,OwnersId,ApproveId,StatusId")] Tasks tasks)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tasks);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // query operator 
            var name_list = from m in _context.UserPer
                            join n in _context.Owner on m.OwnerId equals n.Id
                            select new { Id = m.OwnerId, name = n.own };

            ViewData["ApproveId"] = new SelectList(name_list.Where(s => s.Id == 2).Distinct(), tasks.ApproveId);
            ViewData["OwnersId"] = new SelectList(name_list.Where(s => s.Id == 1).Distinct(), tasks.OwnersId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "status", tasks.StatusId);
            return View(tasks);
        }

        // GET: Task/Edit/id
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks.FindAsync(id);
            if (tasks == null)
            {
                return NotFound();
            }
            var name_list = from m in _context.UserPer
                            join n in _context.Owner on m.OwnerId equals n.Id
                            select new { Id = m.OwnerId, name = n.own };

            ViewData["ApproveId"] = new SelectList(name_list.Where(s => s.Id == 2).Distinct(),"Id", "name", tasks.ApproveId);
            ViewData["OwnersId"] = new SelectList(name_list.Where(s => s.Id == 1).Distinct(),"Id", "name" ,tasks.OwnersId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "status", tasks.StatusId);
            return View(tasks);
        }

        // POST: Task/Edit/id

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Detail,SDate,DDate,OwnersId,ApproveId,StatusId")] Tasks tasks)
        {
            if (id != tasks.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tasks);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TasksExists(tasks.Id))
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
            var name_list = from m in _context.UserPer
                            join n in _context.Owner on m.OwnerId equals n.Id
                            select new { Id = m.OwnerId, name = n.own };

            ViewData["ApproveId"] = new SelectList(name_list.Where(s => s.Id == 2).Distinct(),  "Id", "name",tasks.ApproveId);
            ViewData["OwnersId"] = new SelectList(name_list.Where(s => s.Id == 1).Distinct(), "Id", "name", tasks.OwnersId);

            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "status", tasks.StatusId);
            return View(tasks);
        }

        // GET: Task/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks
                .Include(t => t.Approve)
                .Include(t => t.Owners)
                .Include(t => t.stat)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tasks == null)
            {
                return NotFound();
            }

            return View(tasks);
        }

        // POST: Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tasks = await _context.Tasks.FindAsync(id);
            _context.Tasks.Remove(tasks);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TasksExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
