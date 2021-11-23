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
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace qwerty.Controllers
{
    public class UserPerController : Controller
    {
        private readonly QwertyContext _context;
        public IConfiguration Configuration { get; }
        public string connString;



        public UserPerController(QwertyContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
            connString = Configuration.GetConnectionString("Default");
        }



        // GET: UserPer
        public async Task<IActionResult> Index()
        {
            List<UserPer> from_store = new List<UserPer>();

            using (var conn = new MySqlConnection(connString))
            {
                var cmd = conn.CreateCommand();
                await conn.OpenAsync();
                cmd.CommandText = "s_GetUserper";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@i_visible", 1);


                var reader = cmd.ExecuteReader();
                if (reader.HasRows == true)
                {
                    foreach (var item in reader)
                    {

                        var row = new UserPer
                        {
                            OwnerId = reader.GetInt16("OwnerId"),
                            Ownerss = new Owner { own = reader.GetString("own") },
                            PermissionsId = reader.GetInt16("PermissionsId"),
                            Permissions = new Permission { permission = reader.GetString("permission") }

                        };
                        from_store.Add(row);

                    }


                }



            }

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
            // foreach (var list in query)
            foreach (var list in from_store)

            {

                if (name != list.Ownerss.own)
                {
                    round += 1;
                    if (round != 1)
                    {
                        dt.Rows.Add(dr);
                    }

                    dr = dt.NewRow();
                    dr["Id"] = list.OwnerId;
                    dr["Name"] = list.Ownerss.own;
                    dr[list.Permissions.permission] = true;
                    name = list.Ownerss.own;
                }
                dr[list.Permissions.permission] = true;
            }
            dt.Rows.Add(dr);
            ViewBag.table = dt;
            // await from_store.ToListAsync()
            return View();


        }

        // GET: UserPer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            List<UserPer> from_store = new List<UserPer>();

            using (var conn = new MySqlConnection(connString))
            {
                var cmd = conn.CreateCommand();
                await conn.OpenAsync();
                cmd.CommandText = "s_GetUserPerbyId";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@i_userId", id);
                cmd.Parameters.AddWithValue("@i_visible", 1);


                var reader = cmd.ExecuteReader();
                if (reader.HasRows == true)
                {
                    foreach (var item in reader)
                    {

                        var row = new UserPer
                        {
                            OwnerId = reader.GetInt16("OwnerId"),
                            Ownerss = new Owner { own = reader.GetString("own") },
                            PermissionsId = reader.GetInt16("PermissionsId"),
                            Permissions = new Permission { permission = reader.GetString("permission") },
                            AvailablePermission = GetDefaultPermission(id)

                        };
                        from_store.Add(row);

                    }
                }
                return View(from_store.FirstOrDefault());
            }



        }

        // GET: UserPer/Create
        public async Task<IActionResult> Create()
        {
            List<Owner> owner_List = new List<Owner>();
            using (var conn = new MySqlConnection(connString))
            {
                var cmd = conn.CreateCommand();
                await conn.OpenAsync();
                cmd.CommandText = "s_GetUser";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@i_visible", 1);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows == true)
                {
                    while (await reader.ReadAsync())
                    {
                        var row = new Owner
                        {
                            Id = reader.GetInt16("Id"),
                            own = reader.GetString("own"),
                        };
                        owner_List.Add(row);
                    }
                }
            }
            ViewData["OwnerId"] = new SelectList(owner_List, "Id", "own");
            var UserModel = new UserPer();
            UserModel.AvailablePermission = GetPermission();
            return View(UserModel);
        }

        // POST: UserPer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserPer userPer)
        {
            List<Owner> owner_List = new List<Owner>();
            userPer.AvailablePermission = GetPermission();
            if (ModelState.IsValid)
            {

                foreach (var item in userPer.SelectedPermission)
                {
                    using (var conn = new MySqlConnection(connString))
                    {
                        var cmd = conn.CreateCommand();
                        await conn.OpenAsync();
                        cmd.CommandText = "s_CreateUserPer";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@i_OwnerId", userPer.OwnerId);
                        cmd.Parameters.AddWithValue("@i_perId", Int16.Parse(item));
                        cmd.Parameters.AddWithValue("@i_visible", 1);
                        var reader = cmd.ExecuteNonQuery();
                        await conn.CloseAsync();
                    }
                }
                using (var conn = new MySqlConnection(connString))
                {
                    var cmd = conn.CreateCommand();
                    await conn.OpenAsync();
                    cmd.CommandText = "s_GetUser";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@i_visible", 1);
                    var reader = cmd.ExecuteReader();
                    if (reader.HasRows == true)
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Owner
                            {
                                Id = reader.GetInt16("Id"),
                                own = reader.GetString("own"),
                            };
                            owner_List.Add(row);
                        }
                    }
                }

            }
            ViewData["OwnerId"] = new SelectList(owner_List, "Id", "own", userPer.OwnerId);
            return RedirectToAction(nameof(Index));
        }
        // GET: UserPer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<UserPer> from_store = new List<UserPer>();

            using (var conn = new MySqlConnection(connString))
            {
                var cmd = conn.CreateCommand();
                await conn.OpenAsync();
                cmd.CommandText = "s_GetUserPerbyId";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@i_userId", id);
                cmd.Parameters.AddWithValue("@i_visible", 1);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows == true)
                {
                    foreach (var item in reader)
                    {
                        var row = new UserPer
                        {
                            OwnerId = reader.GetInt16("OwnerId"),
                            Ownerss = new Owner { own = reader.GetString("own") },
                            PermissionsId = reader.GetInt16("PermissionsId"),
                            Permissions = new Permission { permission = reader.GetString("permission") },
                        };
                        from_store.Add(row);
                    }
                }
            }
            var userPer = from_store.FirstOrDefault();
            if (userPer == null)
            {
                return NotFound();
            }
            userPer.AvailablePermission = GetDefaultPermission(id);
            ViewData["PermissionsId"] = new SelectList(_context.Permission.Where(s => s.visible == 1), "Id", "permission", userPer.PermissionsId);
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
            List<Owner> owner_List = new List<Owner>();
            userPer.AvailablePermission = GetDefaultPermission(id);
            var Selected = userPer.SelectedPermission;
            using (var conn = new MySqlConnection(connString))
            {
                var cmd = conn.CreateCommand();
                await conn.OpenAsync();
                cmd.CommandText = "s_EditUserPer";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@i_OwnerId", id);
                cmd.Parameters.AddWithValue("@i_visible", 1);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows == true)
                {
                    while (await reader.ReadAsync())
                    {
                        var row = new Owner
                        {
                            Id = reader.GetInt16("Id"),
                            own = reader.GetString("own"),
                        };
                        owner_List.Add(row);
                    }
                }
                await conn.CloseAsync();
            }
            if (ModelState.IsValid)
            {
                using (var conn = new MySqlConnection(connString))
                {
                    foreach (var item in Selected)
                    {
                        var cmd = conn.CreateCommand();
                        await conn.OpenAsync();
                        cmd.CommandText = "s_CreateUserPer";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@i_OwnerId", id);
                        cmd.Parameters.AddWithValue("@i_perId", Int16.Parse(item));
                        cmd.Parameters.AddWithValue("@i_visible", 1);
                        var reader = cmd.ExecuteNonQuery();
                        await conn.CloseAsync();
                    }
                }
            }
            ViewBag.OwnerId = new SelectList(owner_List, "Id", "own", userPer.OwnerId);
            return RedirectToAction(nameof(Index));
        }
        // GET: UserPer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<UserPer> UserPer_store = new List<UserPer>();
            using (var conn = new MySqlConnection(connString))
            {
                var cmd = conn.CreateCommand();
                await conn.OpenAsync();
                cmd.CommandText = "s_GetUserPerbyId";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@i_userId", id);
                cmd.Parameters.AddWithValue("@i_visible", 1);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows == true)
                {
                    foreach (var item in reader)
                    {
                        var row = new UserPer
                        {
                            OwnerId = reader.GetInt16("OwnerId"),
                            Ownerss = new Owner { own = reader.GetString("own") },
                            PermissionsId = reader.GetInt16("PermissionsId"),
                            Permissions = new Permission { permission = reader.GetString("permission") },
                            AvailablePermission = GetDefaultPermission(id)

                        };
                        UserPer_store.Add(row);
                    }
                }
            }
            var userPer = UserPer_store.FirstOrDefault();
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
            using (var conn = new MySqlConnection(connString))
            {
                var cmd = conn.CreateCommand();
                await conn.OpenAsync();
                cmd.CommandText = "s_EditUserPer";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@i_visible", 1);
                cmd.Parameters.AddWithValue("@i_OwnerId", Id);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));
        }

        private IList<SelectListItem> GetPermission()
        {
            List<SelectListItem> List_permission = new List<SelectListItem>();

            using (var conn = new MySqlConnection(connString))
            {
                var cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = "s_GetPermission";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@i_visible", 1);


                var reader = cmd.ExecuteReader();
                if (reader.HasRows == true)
                {
                    foreach (var item in reader)
                    {

                        var row = new Permission
                        {
                            permission = reader.GetString("permission"),
                            Id = reader.GetInt16("Id")

                        };
                        List_permission.Add(new SelectListItem { Text = row.permission, Value = row.Id.ToString() });

                    }


                }


            }

            return List_permission;


        }

        private IList<SelectListItem> GetDefaultPermission(int? Id)
        {
            List<SelectListItem> List_DefaultPermisison = new List<SelectListItem>();
            List<UserPer> from_store = new List<UserPer>();
            var permission = _context.Permission.Where(s => s.visible == 1).ToList();
            using (var conn = new MySqlConnection(connString))
            {
                var cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = "s_GetUserPerbyId";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@i_userId", (int)Id);
                cmd.Parameters.AddWithValue("@i_visible", 1);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows == true)
                {
                    foreach (var item in reader)
                    {

                        var row = new UserPer
                        {
                            OwnerId = reader.GetInt16("OwnerId"),
                            Ownerss = new Owner { own = reader.GetString("own") },
                            PermissionsId = reader.GetInt16("PermissionsId"),
                            Permissions = new Permission { permission = reader.GetString("permission") }

                        };
                        from_store.Add(row);

                    }

                    if (Id == null)
                    {
                        return null;
                    }
                    else
                    {
                        for (int i = 0; i < permission.Count(); i++)
                        {
                            var check = from_store.Exists(e => e.PermissionsId == permission[i].Id);
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

                }

                return List_DefaultPermisison;

            }


        }


        private IList<SelectListItem> GetForDelete(int? Id)
        {
            List<SelectListItem> List_ForDelete = new List<SelectListItem>();
            List<UserPer> query = new List<UserPer>();

            using (var conn = new MySqlConnection(connString))

            {
                var cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = "s_GetUserPerbyId";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@i_userId", Id);
                cmd.Parameters.AddWithValue("@i_visible", 1);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        var row = new UserPer
                        {
                            OwnerId = reader.GetInt16("OwnerId"),
                            Ownerss = new Owner { own = reader.GetString("own") },
                            PermissionsId = reader.GetInt16("PermissionsId"),
                            Permissions = new Permission { permission = reader.GetString("permission") },

                        };
                        query.Add(row);
                    }
                }

            }

            var permissions = _context.Permission.Where(s => s.visible == 1).ToList();
            if (Id == null)
            {
                return null;
            }
            else
            {
                for (int i = 0; i < permissions.Count(); i++)
                {
                    var check = query.Exists(e => e.PermissionsId == permissions[i].Id);
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
