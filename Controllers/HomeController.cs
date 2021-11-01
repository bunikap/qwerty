using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using qwerty.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using qwerty.Models;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.IO;
using System.Data;
using System;
using ExcelDataReader;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace qwerty.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        private readonly IWebHostEnvironment _webHostEnvironment;


        private readonly QwertyContext _context;

        public HomeController(QwertyContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public ActionResult Index()
        {

            ViewData["OwnerId"] = new SelectList(_context.Owner, "Id", "own");


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
            List<HomeModel> n = new List<HomeModel>();

            var table_data = _context.Tasks.Where(s => s.OwnersId == OwnerId).OrderBy(s => s.Id).Select(s => new { Title = s.Title, Detail = s.Detail, SDate = s.SDate.ToString("dd/MM/yyyy"), DDate = s.DDate.ToString("dd/MM/yyyy"), ApproveId = s.ApproveId, StatusId = s.StatusId }).Distinct().ToList();
            var c = from x in table_data
                    join j in _context.Status on x.StatusId equals j.Id
                    select (new { Title = x.Title, Detail = x.Detail, SDate = x.SDate.ToString(), DDate = x.DDate.ToString(), ApproveId = x.ApproveId, Status = j.status });
            var v = from C in c
                    join J in _context.Owner on C.ApproveId equals J.Id
                    select (new HomeModel { title = C.Title.ToString(), detail = C.Detail.ToString(), start = C.SDate.ToString(), deadline = C.DDate.ToString(), approver = J.own.ToString(), status = C.Status.ToString() });

            if (v.Count() == 0)
            {
                n.Add(new HomeModel { title = "", detail = "", start = "", deadline = "", approver = "", status = "" });
            }
            else
            {
                foreach (var item in v)
                {
                    n.Add(item);
                }
            }
            var table = new { task = n };
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


        public async Task<IActionResult> Export()

        {

            string sWebRootFolder = _webHostEnvironment.WebRootPath;

            string sFileName = @"Employees.xlsx";

            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);

            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));

            var memory = new MemoryStream();

            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))

            {

                IWorkbook workbook;

                workbook = new XSSFWorkbook();

                ISheet excelSheet = workbook.CreateSheet("employee");

                IRow row = excelSheet.CreateRow(0);

                row.CreateCell(0).SetCellValue("EmployeeId");

                row.CreateCell(1).SetCellValue("EmployeeName");

                row.CreateCell(2).SetCellValue("Age");

                row.CreateCell(3).SetCellValue("Sex");

                row.CreateCell(4).SetCellValue("Designation");

                row = excelSheet.CreateRow(1);

                row.CreateCell(0).SetCellValue(1);
                row.CreateCell(1).SetCellValue("Jack Supreu");

                row.CreateCell(2).SetCellValue(45);

                row.CreateCell(3).SetCellValue("Male");

                row.CreateCell(4).SetCellValue("Solution Architect");


                row = excelSheet.CreateRow(2);

                row.CreateCell(0).SetCellValue(2);

                row.CreateCell(1).SetCellValue("Steve khan");

                row.CreateCell(2).SetCellValue(33);

                row.CreateCell(3).SetCellValue("Male");

                row.CreateCell(4).SetCellValue("Software Engineer");


                row = excelSheet.CreateRow(3);

                row.CreateCell(0).SetCellValue(3);

                row.CreateCell(1).SetCellValue("Romi gill");

                row.CreateCell(2).SetCellValue(25);

                row.CreateCell(3).SetCellValue("FeMale");

                row.CreateCell(4).SetCellValue("Junior Consultant");


                row = excelSheet.CreateRow(4);

                row.CreateCell(0).SetCellValue(4);

                row.CreateCell(1).SetCellValue("Hider Ali");

                row.CreateCell(2).SetCellValue(34);

                row.CreateCell(3).SetCellValue("Male");

                row.CreateCell(4).SetCellValue("Accountant");


                row = excelSheet.CreateRow(5);

                row.CreateCell(0).SetCellValue(5);

                row.CreateCell(1).SetCellValue("Mathew");

                row.CreateCell(2).SetCellValue(48);

                row.CreateCell(3).SetCellValue("Male");

                row.CreateCell(4).SetCellValue("Human Resource");


                workbook.Write(fs);

            }

            using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))

            {

                await stream.CopyToAsync(memory);

            }

            memory.Position = 0;

            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);

        }

    }


}




