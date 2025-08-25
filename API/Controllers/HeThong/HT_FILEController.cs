using API.Common;
using API.Controllers.DanhMuc;
using GCommon;
using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Web;
using OfficeOpenXml;
using System;

namespace API.Controllers.HeThong
{
    [Produces("application/json")]
    [Route("api/HeThong/HT_FILE")]
    [ApiController]
    public class HT_FILEController : BaseController
    {
        public HT_FILEController(GDBContext db, IWebHostEnvironment hostingEnvironment, IConfiguration configuration) : base(db, hostingEnvironment, configuration)
        {
            NhomChucNang = NhomChucNang.QuanTriHeThong;
            NhomQuyen = Resource.QuyenHT_FILE;
        }

        public static List<QUYEN> Permission()
        {
            HT_FILEController mn = new HT_FILEController(null, null, null);
            return mn.QuyenCoBan();
        }

        [HttpGet("{op}")]
        [Authorize("Bearer")]
        public IActionResult Get(string op)
        {
            if (op == "Access")
            {
                return ObjectResult(new
                {
                    View = UserAccess(Quyen.Xem),
                    New = UserAccess(Quyen.Them),
                    Edit = UserAccess(Quyen.Sua),
                    Delete = UserAccess(Quyen.Xoa),
                });
            }
            else if (op == "Folder")
            {
                string subDir = Request.Query["PATH"].ToString();
                if (subDir + "" == "") subDir = "\\Uploads";
                string webRootPath = GetRootPath;
                string pathUpload = webRootPath + subDir;
                if (!Directory.Exists(pathUpload)) Directory.CreateDirectory(pathUpload);
                List<FOLDERTREE> folderTree = new List<FOLDERTREE>();
                LoadSubDirs(folderTree, pathUpload, "", webRootPath);
                return ListAll(folderTree.AsQueryable());
            }
            else if (op == "List")
            {
                string subDir = Request.Query["PATH"].ToString();
                if (subDir + "" == "") subDir = "\\Uploads";
                string webRootPath = GetRootPath;
                string pathUpload = webRootPath + subDir;
                if (!Directory.Exists(pathUpload)) Directory.CreateDirectory(pathUpload);
                List<FILETREE> fileTree = new List<FILETREE>();
                string[] listFiles = Directory.GetFiles(pathUpload);
                foreach (string f in listFiles)
                {
                    var fInfo = new FileInfo(f);
                    var ext = Path.GetExtension(f);
                    int w = 0, h = 0;
                    if (ext == ".jpg" || ext == ".jpge" || ext == ".png" || ext == ".gif")
                    {
                        var img = Image.Load(f);
                        w = img.Width;
                        h = img.Height;
                    }
                    fileTree.Add(new FILETREE { PATH = f.Replace(webRootPath, ""), FILENAME = fInfo.Name, FILESIZE = fInfo.Length, CREATED = fInfo.CreationTime, WIDTH = w, HEIGHT = h });
                }
                return ListAll(fileTree.AsQueryable());
            }
            else if (op == "ReadExcell")
            {
                string path = Request.Query["PATH"].ToString();
                string type = Request.Query["TYPE"].ToString();
                string filePath = GetRootPath + path;
                if (System.IO.File.Exists(filePath))
                {
                    FileInfo file = new FileInfo(filePath);
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    List<object> data = new List<object>();
                    Dictionary<int, string> colName = new Dictionary<int, string>();
                    using (ExcelPackage package = new ExcelPackage(file))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        int rowCount = worksheet.Dimension.End.Row;
                        int colCount = worksheet.Dimension.End.Column;
                        for (int col = 1; col <= colCount; col++)
                        {
                            string value = worksheet.Cells[1, col].Value + "";
                            colName.Add(col, value);
                        }
                        for (int row = 2; row <= rowCount; row++)
                        {
                            JObject rData = new JObject();
                            for (int col = 1; col <= colCount; col++)
                            {
                                string value = "";
                                if (type == "MAU_TAILIEU" && (col == 8 || col == 9 || col == 10 || col == 11 || col == 22 || col == 24 || col == 26))
                                {
                                    if (worksheet.Cells[row, col].Value + "" != "")
                                    {
            
                                        string inputDate = worksheet.Cells[row, col].Value + "";
                                        string cleanedDate = inputDate.TrimStart().Replace("\u00A0", "").Replace("\t", "");
                                        value = ConvertClass.ExcelToDateTime(cleanedDate).ToString();
                                    }
                                }
                                else if (type == "DM_CANBO" && (col == 3 || col == 8))
                                {
                                    if (worksheet.Cells[row, col].Value + "" != "") value = ConvertClass.ExcelToDateTime(worksheet.Cells[row, col].Value + "").ToString();
                                }
                                else
                                {
                                    value = worksheet.Cells[row, col].Value + "";
                                }
                                if (value == "") continue;
                                rData[col.ToString()] = value;
                               
                                
                            }
                            if (rData.Count <= 0) continue;
                            data.Add(rData);
                        }
                    }
                    System.IO.File.Delete(filePath);
                    return ObjectResult(new { colName, data });
                }
            }
            return new BadRequestResult();
        }

        private void LoadSubDirs(List<FOLDERTREE> folderTree, string dir, string subDir, string rpath)
        {
            string[] subdirectoryEntries = Directory.GetDirectories(dir + "" != "" ? dir : subDir);
            foreach (string subdirectory in subdirectoryEntries)
            {
                folderTree.Add(new FOLDERTREE { PATH = subdirectory.Replace(rpath, ""), NAME = Path.GetFileName(subdirectory), PID = subDir.Replace(rpath, "") });
                LoadSubDirs(folderTree, "", subdirectory, rpath);
            }
        }

        [HttpPost("Folder/{op}")]
        [Authorize("Bearer")]
        public IActionResult PostFolder(string op, [FromBody] JObject item)
        {
            if (item == null || op == "") return new BadRequestResult();
            string webRootPath = GetRootPath;
            if (op == "Create")
            {
                //if (!UserAccess("Tạo folder file")) return UserAccessDenied();
                Directory.CreateDirectory(Path.Combine(webRootPath + item["PATH"], item["NAME"] + ""));
            }
            else if (op == "Delete")
            {
                //if (!UserAccess("Xoá folder file")) return UserAccessDenied();
                Directory.Delete(webRootPath + item["PATH"], true);
            }
            return new NoContentResult();
        }

        [HttpPost("File/{PATH}"), DisableRequestSizeLimit]
        [Authorize("Bearer")]
        public IActionResult PostFile(string PATH)
        {
            try
            {
                PATH = HttpUtility.UrlDecode(PATH);
                var file = Request.Form.Files[0];
                string pathUpload = GetRootPath + PATH;
                string pathUploadSV = "";
                if (!Directory.Exists(pathUpload)) Directory.CreateDirectory(pathUpload);
                if (file.Length > 0)
                {
                    string fileFullName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fileNameOrig = Path.GetFileNameWithoutExtension(fileFullName).CleanFileName();
                    string fileName = fileNameOrig;
                    string fileExt = Path.GetExtension(fileFullName);
                    int fileNameCount = 0;
                    string fullPath = "";
                    do
                    {
                        fileName = fileNameOrig + (fileNameCount > 0 ? fileNameCount.ToString() : "") + fileExt;
                        pathUploadSV = Path.Combine(PATH, fileName);
                        fullPath = Path.Combine(pathUpload, fileName);
                        fileNameCount++;
                    } while (fullPath == "" || System.IO.File.Exists(fullPath));
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    FileInfo item = new FileInfo(fullPath);
                    return ObjectResult(new { item.Length, item.Name, item.Extension, Path = pathUploadSV });
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(new RequestResult
                {
                    State = RequestState.Success,
                    Msg = ex.Message,
                });
            }
            return new NoContentResult();
        }

        [HttpPost("File/Delete")]
        [Authorize("Bearer")]
        public IActionResult PostDelete([FromBody] JObject item)
        {
            if (!UserAccess(Quyen.Xoa)) return UserAccessDenied();
            try
            {
                string webRootPath = GetRootPath;
                var jItem = JsonConvert.DeserializeObject<JArray>(item.GetValue("items").Value<string>());
                foreach (var fid in jItem)
                {
                    string fName = webRootPath + fid;
                    if (System.IO.File.Exists(fName)) System.IO.File.Delete(fName);
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(new RequestResult
                {
                    State = RequestState.Success,
                    Msg = ex.Message,
                });
            }
            return new NoContentResult();
        }
    }
}
