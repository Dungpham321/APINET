using API.Common;
using Azure.Core;
using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using System.IO;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Web;
using GCommon;

namespace API.Controllers.HeThong
{
    [Produces("application/json")]
    [Route("api/HeThong/HT_TEP")]
    [ApiController]
    public class HT_TEPController : BaseController
    {
        public HT_TEPController(GDBContext db, IWebHostEnvironment hostingEnvironment, IConfiguration configuration) : base(db, hostingEnvironment, configuration)
        {
            NhomChucNang = NhomChucNang.QuanTriHeThong;
            NhomQuyen = Resource.QuyenHT_TEP;
        }

        [HttpGet("{op}")]
        [Authorize("Bearer")]
        public IActionResult Get(string op)
        {
            
            if (op == "List")
            {
                long DOITUONG_ID = Convert.ToInt64(Request.Query["DOITUONG_ID"]);
                string CHUCNANG = Request.Query["CHUCNANG"] + "";
                string DOITUONG_LOAI = Request.Query["DOITUONG_LOAI"] + "";
                List<long> lstIDS = new List<long>();
                if (DOITUONG_LOAI != "")
                {
                    lstIDS = _db.HT_TEP_SDCollection.Get(DOITUONG_ID, CHUCNANG, DOITUONG_LOAI).Select(s => s.TEP_ID.Value).ToList();
                }
                else
                {
                    lstIDS = _db.HT_TEP_SDCollection.Get(DOITUONG_ID, CHUCNANG).Select(s => s.TEP_ID.Value).ToList();
                }
                if (lstIDS.Count == 0) return ObjectResult(new List<HT_TEP>());
                if (DOITUONG_ID == 0)
                {
                    return ObjectResult(_db.HT_TEPCollection.Get(lstIDS).Where(c => c.NGUOIDUNG_ID == CurrentUser.ID));
                }
                else{
                    return ObjectResult(_db.HT_TEPCollection.Get(lstIDS).ToList());
                }
            }
            else if (op == "ListInfo")
            {
                long DOITUONG_ID = Convert.ToInt64(Request.Query["DOITUONG_ID"]);
                string CHUCNANG = Request.Query["CHUCNANG"] + "";
                string DOITUONG_LOAI = Request.Query["DOITUONG_LOAI"] + "";
                List<long> lstIDS = new List<long>();
                if (DOITUONG_LOAI != "")
                {
                    lstIDS = _db.HT_TEP_SDCollection.Get(DOITUONG_ID, CHUCNANG, DOITUONG_LOAI).Select(s => s.TEP_ID.Value).ToList();
                }
                else
                {
                    lstIDS = _db.HT_TEP_SDCollection.Get(DOITUONG_ID, CHUCNANG).Select(s => s.TEP_ID.Value).ToList();
                }
                if (lstIDS.Count == 0) return ObjectResult(new List<HT_TEP>());
                if (DOITUONG_ID == 0)
                {
                    return ObjectResult(_db.HT_TEPCollection.GetInfo(lstIDS).Where(c => c.NGUOIDUNG_ID == CurrentUser.ID));
                }
                else
                {
                    return ObjectResult(_db.HT_TEPCollection.GetInfo(lstIDS));
                }
            }
            else if (op == "Download")
            {
                long ID = Convert.ToInt64(Request.Query["ID"]);
                HT_TEP objHT_TEP = AccessFile(ID);
                if (objHT_TEP == null) return new NoContentResult();
                string filePath = GetRootPath + objHT_TEP.DUONG_DAN;
                try
                {
                    Byte[] bytes = System.IO.File.ReadAllBytes(filePath);
                    String file = "data:" + objHT_TEP.KIEU_TEP.GetMimeTypeByWindowsRegistry() + ";base64," + Convert.ToBase64String(bytes);
                    return ObjectResult(file, objHT_TEP.TEN_TEP);
                }
                catch
                {
                    return new BadRequestResult();
                }
            }
            else if (op == "View")
            {
                long ID = Convert.ToInt64(Request.Query["ID"]);
                HT_TEP objHT_TEP = AccessFile(ID);
                if (objHT_TEP == null) return new NoContentResult();
                string filePath = GetRootPath + objHT_TEP.DUONG_DAN;
                try
                {
                    if((new string[] {".doc", ".docx", ".xls", ".xlsx", ".pdf", ".jpg", ".png", ".gif", ".jpeg" }).Contains(objHT_TEP.KIEU_TEP.ToLower()))
                    {
                        Byte[] bytes = System.IO.File.ReadAllBytes(filePath);
                        Dictionary<string, object> viewData = new Dictionary<string, object>();
                        viewData.Add("datafile", Convert.ToBase64String(bytes));
                        viewData.Add("fileext", objHT_TEP.KIEU_TEP);
                        viewData.Add("filemine", objHT_TEP.KIEU_TEP.GetMimeTypeByWindowsRegistry());
                        return ObjectResult(new { token = CreateToken(), data = JsonConvert.SerializeObject(viewData).Base64Encode() });
                    }
                    else
                    {
                        return ObjectResult(null, Resource.strKhongHoTroXem);
                    }
                }
                catch
                {
                    return new BadRequestResult();
                }
            }
            else if (op == "GiaiNen")
            {
                long ID = Convert.ToInt64(Request.Query["ID"]);
                string DOITUONG_LOAI = Request.Query["DOITUONG_LOAI"];
                string CHUCNANG = Request.Query["CHUCNANG"];
                HT_TEP objHT_TEP = AccessFile(ID);
                if (objHT_TEP == null) return new NoContentResult();
                string filePath = GetRootPath + objHT_TEP.DUONG_DAN;
                try
                {
                    if(objHT_TEP.KIEU_TEP == ".zip")
                    {
                        string folder = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));
                        if (Directory.Exists(folder)) RemoveFolder(folder);
                        else Directory.CreateDirectory(folder);
                        using (var zip = ZipFile.OpenRead(filePath))
                        {
                            zip.ExtractToDirectory(folder);
                            DirectoryInfo dir = new DirectoryInfo(folder);
                            foreach (FileInfo fileimport in dir.GetFiles("*.*"))
                            {
                                //save file
                                HT_TEP objHT_TEP_NEW = new HT_TEP();
                                objHT_TEP_NEW.TEN_TEP = fileimport.Name;
                                objHT_TEP_NEW.TEN_BAN_DAU = fileimport.Name;
                                objHT_TEP_NEW.DUONG_DAN = Path.Combine(folder, fileimport.Name).Replace(GetRootPath, "");
                                objHT_TEP_NEW.KIEU_TEP = fileimport.Extension;
                                objHT_TEP_NEW.KICH_THUOC = fileimport.Length;

                                objHT_TEP_NEW.NGAY_TAO = DateTime.Now;
                                objHT_TEP_NEW.NGUOIDUNG_ID = CurrentUser.ID;
                                objHT_TEP_NEW.TRANG_THAI = (int)TrangThaiTep.DANG_SU_DUNG;
                                _db.HT_TEPCollection.Add(objHT_TEP_NEW);

                                HT_TEP_SD objHT_TEP_SD = new HT_TEP_SD();
                                objHT_TEP_SD.TEP_ID = objHT_TEP_NEW.ID;
                                objHT_TEP_SD.DOITUONG_ID = CurrentUser.ID;
                                objHT_TEP_SD.DOITUONG_LOAI = DOITUONG_LOAI;
                                objHT_TEP_SD.CHUCNANG = CHUCNANG;
                                _db.HT_TEP_SDCollection.Add(objHT_TEP_SD);
                            }    
                        }
                    }
                    return Delete(ID);
                }
                catch
                {
                    return new BadRequestResult();
                }
            }
            else if (op == "Remove")
            {
                long DOITUONG_ID = Convert.ToInt64(Request.Query["DOITUONG_ID"]);
                string DOITUONG_LOAI = Request.Query["DOITUONG_LOAI"];
                string CHUCNANG = Request.Query["CHUCNANG"];
                RemoveFile(DOITUONG_ID, CHUCNANG, DOITUONG_LOAI);
                return new NoContentResult();
            }
            else if (op == "GetFileSign")
            {
                long ID = Convert.ToInt64(Request.Query["ID"]);
                HT_TEP objHT_TEP = AccessFile(ID);
                if (objHT_TEP == null) return new NoContentResult();
                string filePath = GetRootPath + objHT_TEP.DUONG_DAN;
                try
                {
                    if ((new string[] { ".pdf" }).Contains(objHT_TEP.KIEU_TEP))
                    {
                        Byte[] bytes = System.IO.File.ReadAllBytes(filePath);
                        var domain = HttpContext.Request.Scheme + "://" +  HttpContext.Request.Host.Value;
                        domain = domain + _configuration.GetValue<string>("AppSetings:SUB_PATH") + "";
                        return ObjectResult(domain + objHT_TEP.DUONG_DAN.Replace("\\", "/"));
                    }
                    else
                    {
                        return ObjectResult(null, Resource.strKhongHoTroKySo);
                    }
                }
                catch
                {
                    return new BadRequestResult();
                }
            }
            else if (op == "UpdateFileSign")
            {
                long ID = Convert.ToInt64(Request.Query["ID"]);
                string FileServer = Request.Query["FileServer"] + "";
                if (FileServer == "") return BadRequest();
                HT_TEP objHT_TEP = AccessFile(ID);
                if (objHT_TEP == null) return new NoContentResult();

                //xoa file cu
                string filePath = GetRootPath + objHT_TEP.DUONG_DAN;
                try
                {
                    if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                }
                catch { }
                //di chuyen file
                string newFile = GetRootPath + FileServer;
                System.IO.File.Move(newFile, filePath);
                objHT_TEP.KY_SO = true;
                _db.HT_TEPCollection.Update(objHT_TEP);
                return new NoContentResult();
            }
            return new BadRequestResult();
        }

        [HttpPost("Uploaded")]
        [Authorize("Bearer")]
        public IActionResult Uploaded([FromBody] JObject item)
        {
            HT_TEP_SD objHT_TEP_SD = item.ToObject<HT_TEP_SD>();
            if (!objHT_TEP_SD.DOITUONG_ID.HasValue) objHT_TEP_SD.DOITUONG_ID = 0;
            objHT_TEP_SD.ND_ID = CurrentUser.ID;
            _db.HT_TEP_SDCollection.Add(objHT_TEP_SD);
            if(objHT_TEP_SD.DOITUONG_ID > 0)
            {
                HT_TEP objHT_TEP = _db.HT_TEPCollection.GetByID(objHT_TEP_SD.TEP_ID.Value);
                if(objHT_TEP != null)
                {
                    objHT_TEP.TRANG_THAI = (int)TrangThaiTep.DANG_SU_DUNG;
                    _db.HT_TEPCollection.Update(objHT_TEP);
                }
            }            
            return new NoContentResult();
        }

        [HttpPost("Upload/{PATH}"), DisableRequestSizeLimit]
        [Authorize("Bearer")]
        public IActionResult PostFile(string PATH)
        {
            try
            {
                PATH = HttpUtility.UrlDecode(PATH);
                var file = Request.Form.Files[0];
                string pathUpload = GetRootPath + PATH;
                //string pathUploadSV = "Uploads";
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
                    //save file
                    HT_TEP objHT_TEP = new HT_TEP();
                    objHT_TEP.TEN_TEP = fileFullName;
                    objHT_TEP.TEN_BAN_DAU = item.Name;
                    objHT_TEP.DUONG_DAN = pathUploadSV;
                    objHT_TEP.KIEU_TEP = item.Extension;
                    objHT_TEP.KICH_THUOC = item.Length;

                    objHT_TEP.NGAY_TAO = DateTime.Now;
                    objHT_TEP.NGUOIDUNG_ID = CurrentUser.ID;
                    objHT_TEP.TRANG_THAI = (int)TrangThaiTep.KHONG_SU_DUNG;
                    _db.HT_TEPCollection.Add(objHT_TEP);


                    HtLog(Quyen.Them, objHT_TEP.ID);
                    return ObjectResult(new { objHT_TEP.ID });
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

        [HttpPost("UploadSign"), DisableRequestSizeLimit]
        [Authorize("Bearer")]
        public IActionResult UploadSign()
        {
            try
            {
                var file = Request.Form.Files[0];
                string pathUpload = GetRootPath + Resource.strFolderSign;
                if (!Directory.Exists(pathUpload)) Directory.CreateDirectory(pathUpload);
                string pathUploadSV = "";
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
                        pathUploadSV = Path.Combine(Resource.strFolderSign, fileName);
                        fullPath = Path.Combine(pathUpload, fileName);
                        fileNameCount++;
                    } while (fullPath == "" || System.IO.File.Exists(fullPath));
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    FileInfo item = new FileInfo(fullPath);
                    return new ObjectResult(new
                    {
                        Status = true,
                        Message = "",
                        FileName = item.Name,
                        FileServer = pathUploadSV
                    });
                }                
            }
            catch (Exception ex)
            {
                return new ObjectResult(new
                {
                    Status = false,
                    Message = ex.Message,
                    FileName = "",
                    FileServer = ""
                });
            }
            return new ObjectResult(new
            {
                Status = false,
                Message = "",
                FileName = "",
                FileServer = ""
            });
        }

        [HttpDelete("Delete/{ID}")]
        [Authorize("Bearer")]
        public IActionResult Delete(long ID)
        {
            HT_TEP objHT_TEP = _db.HT_TEPCollection.GetByID(ID);
            if (objHT_TEP == null) return new NoContentResult();
            if (objHT_TEP.NGUOIDUNG_ID != CurrentUser.ID) return new BadRequestResult();
            _db.HT_TEPCollection.Remove(objHT_TEP);
            _db.HT_TEP_SDCollection.RemoveByTEP_ID(objHT_TEP.ID);
            string filePath = GetRootPath + objHT_TEP.DUONG_DAN;
            try
            {
                if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
            }
            catch {}
            HtLog(Quyen.Xoa, objHT_TEP.ID);
            return new NoContentResult();
        }

        [HttpDelete("Delete/{ID}/{DOITUONG_ID}/{CHUCNANG}/{DOITUONG_LOAI}")]
        [Authorize("Bearer")]
        public IActionResult DeleteF(long ID, long DOITUONG_ID, string CHUCNANG, string DOITUONG_LOAI = "")
        {
            HT_TEP objHT_TEP = _db.HT_TEPCollection.GetByID(ID);
            if (objHT_TEP == null) return new NoContentResult();
            if (objHT_TEP.NGUOIDUNG_ID != CurrentUser.ID) return new BadRequestResult();

            var lstHT_TEP_SD = _db.HT_TEP_SDCollection.Get(ID, DOITUONG_ID, CHUCNANG, DOITUONG_LOAI).ToList();
            foreach (var objHT_TEP_SD in lstHT_TEP_SD)
            {
                _db.HT_TEP_SDCollection.Remove(objHT_TEP_SD);
            }

            var checkSD = _db.HT_TEP_SDCollection.GetByTEP_ID(ID).ToList();
            if (checkSD.Count == 0)
            {
                _db.HT_TEPCollection.Remove(objHT_TEP);
                _db.HT_TEP_SDCollection.RemoveByTEP_ID(objHT_TEP.ID);
                string filePath = GetRootPath + objHT_TEP.DUONG_DAN;
                try
                {
                    if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                }
                catch { }
                HtLog(Quyen.Xoa, objHT_TEP.ID);
            }
            return new NoContentResult();
        }

        //DungPV
        [HttpDelete("DeleteF/{DOITUONG_ID}/{CHUCNANG}/{DOITUONG_LOAI}")]
        [Authorize("Bearer")]
        public IActionResult DeleteFF(long DOITUONG_ID, string CHUCNANG, string DOITUONG_LOAI = "")
        {
            var lstHT_TEP_SD = _db.HT_TEP_SDCollection.Get(DOITUONG_ID, CHUCNANG, DOITUONG_LOAI).ToList();
            foreach (var objHT_TEP_SD in lstHT_TEP_SD)
            {
                _db.HT_TEP_SDCollection.Remove(objHT_TEP_SD);
                HT_TEP objHT_TEP = _db.HT_TEPCollection.GetByID((long)objHT_TEP_SD.TEP_ID);
                _db.HT_TEPCollection.Remove(objHT_TEP);
                string filePath = GetRootPath + objHT_TEP.DUONG_DAN;
                try
                {
                    if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                }
                catch { }
                HtLog(Quyen.Xoa, objHT_TEP.ID);
            }
            return new NoContentResult();
        }
        //END

        [HttpPost("ReName")]
        [Authorize("Bearer")]
        public IActionResult ReName([FromBody] JObject item)
        {
            HT_TEP objHT_TEP = _db.HT_TEPCollection.GetByID(Convert.ToInt64(item["TEP_ID"]));
            if (objHT_TEP != null && item["TEN_TEP"] + "" != "")
            {
                objHT_TEP.TEN_TEP = item["TEN_TEP"] + "";
                if (item["TGROUP"] + "" != "") objHT_TEP.TGROUP = item["TGROUP"] + "";
                else objHT_TEP.TGROUP = null;
                _db.HT_TEPCollection.Update(objHT_TEP);
            }
            return new NoContentResult();
        }
    }
}
