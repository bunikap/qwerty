using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using qwerty.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using qwerty.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Data;
using System;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using MySql.Data.MySqlClient;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace qwerty.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly QwertyContext _context;
        public IConfiguration Configuration { get; }
        public string connString;



        public HomeController(QwertyContext context, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            Configuration = configuration;
            connString = Configuration.GetConnectionString("Default");

        }
        public ActionResult Index()
        {
            List<Owner> owner_list = new List<Owner>();
            using (var conn = new MySqlConnection(connString))
            {
                var cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = "s_GetUser";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@i_visible", 1);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows == true)
                {
                    foreach (var item in reader)
                    {
                        var row = new Owner
                        {
                            Id = reader.GetInt16("Id"),
                            own = reader.GetString("own")
                        };
                        owner_list.Add(row);
                    }
                }
            }
            ViewData["OwnerId"] = new SelectList(owner_list, "Id", "own");
            return View();
        }
        [HttpPost]
        // public JsonResult passIntoView(int OwnerId)
        public JsonResult passIntoView(int OwnerId)
        {
            List<string> label = new List<string>();
            List<double> value = new List<double>();
            var status_list = _context.Status.ToList();
            var OwnerTask_List = _context.Tasks.Where(s => s.OwnersId == OwnerId).ToList();
            var query = from status in status_list
                        join own_task in OwnerTask_List on status.Id equals own_task.StatusId into gj
                        select new { status = status.status, Task = gj.Count() };
            double sum = 0;
            foreach (var item in query)
            {
                sum += item.Task;
            }
            foreach (var item in query)
            {
                if (sum == 0)
                {
                    label.Add(item.status);
                    value.Add(0);
                }
                else
                {
                    double percentage = (item.Task * 100 / sum);
                    label.Add(item.status);
                    value.Add(percentage);
                }
            }
            var Data_points = new
            {
                labels = label,
                series = value,
            };
            // ViewBag.Data = Data_points;
            return Json(Data_points);

        }
        [HttpPost]
        public JsonResult DataTable(int OwnerId)
        {
            List<HomeModel> from_store = new List<HomeModel>();
            using (var conn = new MySqlConnection(connString))
            {
                var cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = "s_GetHomeModel";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@i_OwnerId", OwnerId);
                cmd.Parameters.AddWithValue("@i_visible", 1);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows == true)
                {
                    foreach (var item in reader)
                    {
                        var row = new HomeModel
                        {
                            title = reader.GetString("Title"),
                            detail = reader.GetString("Detail"),
                            start = reader.GetDateTime("SDate").ToShortDateString(),
                            deadline = reader.GetDateTime("DDate").ToShortDateString(),
                            approver = reader.GetString("own"),
                            status = reader.GetString("status"),
                        };
                        from_store.Add(row);
                    }
                }

            }
            if (from_store.Count() == 0)
            {
                from_store.Add(new HomeModel { title = "", detail = "", start = "", deadline = "", approver = "", status = "" });
            }
            var table = new { task = from_store };
            return Json(table);
        }
        public async Task<IActionResult> Import()
        {
            IFormFile file = Request.Form.Files[0];
            string folderName = "UploadExcel";
            string webRootPath = _webHostEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            // var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            if (file.Length > 0)
            {
                string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                ISheet sheet;
                string fullPath = Path.Combine(newPath, file.FileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    stream.Position = 0;
                    if (sFileExtension == ".xls")
                    {
                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  
                    }
                    else
                    {
                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   
                    }
                    IRow headerRow = sheet.GetRow(0); //Get Header Row            
                    List<string> head = new List<string>();


                    int cellCount = headerRow.LastCellNum;
                    List<List<string>> cells = new List<List<string>>();
                    for (int j = 0; j < cellCount; j++)
                    {
                        NPOI.SS.UserModel.ICell cell = headerRow.GetCell(j);
                        if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                        head.Add(cell.ToString());
                    }


                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        List<string> rowData = new List<string>();
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {

                            if (row.GetCell(j) != null)
                            {
                                rowData.Add(row.GetCell(j).ToString());
                            }

                        }
                        cells.Add(rowData);
                    }

                    foreach (var i in cells)
                    {
                        var a = new Tasks { Title = i[0], Detail = i[1], SDate = Convert.ToDateTime(i[2]), DDate = Convert.ToDateTime(i[3]), OwnersId = int.Parse(i[4]), ApproveId = int.Parse(i[5]), StatusId = int.Parse(i[6]) };
                        _context.Add(a);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            return View();

        }


        public ActionResult Download()

        {
            string Files = "wwwroot/UploadExcel/example.xlsx";

            byte[] fileBytes = System.IO.File.ReadAllBytes(Files);

            System.IO.File.WriteAllBytes(Files, fileBytes);

            MemoryStream ms = new MemoryStream(fileBytes);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "example.xlsx");

        }


    }


}




