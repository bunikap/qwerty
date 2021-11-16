using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using qwerty.Data;
using qwerty.Models;

namespace qwerty.Controllers
{
    public class UserPerController : Controller
    {
        private readonly QwertyContext _context;

        public UserPerController(QwertyContext context)
        {
            _context = context;
        }

        // GET: UserPer
        public async Task<IActionResult> Index()
        {
            var qwertyContext = _context.UserPer.Include(u => u.Ownerss).Include(u => u.Permissions).Where(s=>s.visible==1);
        
            var query = qwertyContext.Select(s => new { UserId = s.OwnerId, User = s.Ownerss.own, Permission = s.PermissionsId, per = s.Permissions.permission }).Distinct().ToList();
            var permission_list = _context.Permission.ToList();
            DataTable dt = new DataTable();
            dt.Clear();
            dt.Columns.Add("Id");
            dt.Columns.Add("Name");
            foreach (var i in permission_list)
            {
                dt.Columns.Add(i.permission, typeof(bool));
            }
            int round = 0;
            string name = "";
            DataRow dr = dt.NewRow();
            foreach (var list in query)
            {
                if (name != list.User)
                {
                    round += 1;
                    if (round != 1)
                    {
                        dt.Rows.Add(dr);
                    }

                    dr = dt.NewRow();
                    dr["Id"] = list.UserId;
                    dr["Name"] = list.User;
                    dr[list.per] = true;
                    name = list.User;
                }
                dr[list.per] = true;
            }
            dt.Rows.Add(dr);
            ViewBag.table = dt;
            return View(await qwertyContext.ToListAsync());
        }

        // GET: UserPer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userPer = await _context.UserPer
                .Include(u => u.Ownerss)
                .Include(u => u.Permissions).Where(s =>s.visible==1)
                .FirstOrDefaultAsync(m => m.OwnerId == id);
            userPer.AvailablePermission = GetDefaultPermission(id);
            if (userPer == null)
            {
                return NotFound();
            }
            return View(userPer);
        }

        // GET: UserPer/Create
        public IActionResult Create()
        {
            ViewData["OwnerId"] = new SelectList(_context.Owner.Where(s => s.visible==1), "Id", "own");
            var UserModel = new UserPer();
            UserModel.AvailablePermission = GetPermission();
            return View(UserModel);
        }

        // POST: UserPer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserPer userPer)
        {
            userPer.AvailablePermission = GetPermission();
            if (ModelState.IsValid)
            {
                foreach (var item in userPer.SelectedPermission)
                {
                    var User = new UserPer { Id = userPer.Id, OwnerId = userPer.OwnerId, PermissionsId = Int16.Parse(item), visible=1 };
                    _context.Add(User);
                    await _context.SaveChangesAsync();
                }
            }
            ViewData["OwnerId"] = new SelectList(_context.Owner.Where(s => s.visible==1), "Id", "own", userPer.OwnerId);
            return RedirectToAction(nameof(Index));
        }
        // GET: UserPer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userPer = await _context.UserPer
                .Include(u => u.Ownerss)
                .Include(u => u.Permissions).Where(s =>s.visible==1)
                .FirstOrDefaultAsync(m => m.OwnerId == id);

            userPer.AvailablePermission = GetDefaultPermission(id);
            if (userPer == null)
            {
                return NotFound();
            }
            ViewData["PermissionsId"] = new SelectList(_context.Permission.Where(s =>s.visible==1), "Id", "permission", userPer.PermissionsId);
            return View(userPer);
        }

        // POST: UserPer/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, UserPer userPer)
        {
            if (id == null)
            {
                return NotFound();
            }
            userPer.AvailablePermission = GetDefaultPermission(id);
            var Selected = userPer.SelectedPermission;
            var User_delete = _context.UserPer.Where(x => x.OwnerId == id).Where(s => s.visible==1).ToList();
            foreach (var item in User_delete)
            {
                item.visible=0;
                _context.UserPer.Update(item);
                await _context.SaveChangesAsync();
            }
            if (ModelState.IsValid)
            {
                foreach (var item in Selected)
                {
                    var User = new UserPer { OwnerId = (int)id, PermissionsId = Int16.Parse(item), visible=1 };
                    _context.Add(User);
                    await _context.SaveChangesAsync();
                }
            }
            ViewBag.OwnerId = new SelectList(_context.Owner, "Id", "own", userPer.OwnerId);
            return RedirectToAction(nameof(Index));
        }
        // GET: UserPer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userPer = await _context.UserPer
                .Include(u => u.Ownerss)
                .Include(u => u.Permissions).Where(s =>s.visible==1)
                .FirstOrDefaultAsync(m => m.OwnerId == id);
            userPer.AvailablePermission = GetForDelete(id);
            if (userPer == null)
            {
                return NotFound();
            }
            return View(userPer);
        }

        // POST: UserPer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int Id)
        {
            var userPer = _context.UserPer.Where(x => x.OwnerId == Id).ToList();
            foreach (var item in userPer)
            {
                item.visible=0;
                _context.UserPer.Update(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private IList<SelectListItem> GetPermission()
        {
            List<SelectListItem> List_permission = new List<SelectListItem>();
            var AllPermission = _context.Permission.Where(s =>s.visible==1).ToList();
            for (int i = 0; i < AllPermission.Count(); i++)
            {
                List_permission.Add(new SelectListItem { Text = AllPermission[i].permission, Value = AllPermission[i].Id.ToString() });
            }
            return List_permission;
        }

        private IList<SelectListItem> GetDefaultPermission(int? Id)
        {
            List<SelectListItem> List_DefaultPermisison = new List<SelectListItem>();
            var qwertyContext = _context.UserPer.Include(u => u.Ownerss).Include(u => u.Permissions).Where(s =>s.visible==1);
            var query = qwertyContext.Select(s => new { UserId = s.OwnerId, User = s.Ownerss.own, Permission = s.PermissionsId, per = s.Permissions.permission }).Distinct().Where(s => s.UserId == Id).ToList();
            var permission = _context.Permission.ToList();
            if (Id == null)
            {
                return null;
            }
            else
            {
                for (int i = 0; i < permission.Count(); i++)
                {
                    var check = query.Exists(e => e.Permission == permission[i].Id);
                    if (check == true)
                    {
                        List_DefaultPermisison.Add(new SelectListItem { Text = permission[i].permission, Value = permission[i].Id.ToString(), Selected = true });
                    }
                    else
                    {
                        List_DefaultPermisison.Add(new SelectListItem { Text = permission[i].permission, Value = permission[i].Id.ToString(), Selected = false });
                    }
                }

            }
            return List_DefaultPermisison;
        }


        private IList<SelectListItem> GetForDelete(int? Id)
        {
            List<SelectListItem> List_ForDelete = new List<SelectListItem>();
            var qwertyContext = _context.UserPer.Include(u => u.Ownerss).Include(u => u.Permissions).Where(s=>s.visible==1);
            var query = qwertyContext.Select(s => new { UserId = s.OwnerId, User = s.Ownerss.own, Permission = s.PermissionsId, per = s.Permissions.permission }).Distinct().Where(s => s.UserId == Id).ToList();
            var permissions = _context.Permission.Where(s=>s.visible==1).ToList();
            if (Id == null)
            {
                return null;
            }
            else
            {
                for (int i = 0; i < permissions.Count(); i++)
                {
                    var check = query.Exists(e => e.Permission == permissions[i].Id);
                    if (check == true)
                    {
                        List_ForDelete.Add(new SelectListItem { Text = permissions[i].permission, Value = permissions[i].Id.ToString(), Selected = true });
                    }

                }
            }
            return List_ForDelete;
        }
    }
}
