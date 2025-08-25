using API.Common;
using GCommon;
using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace API.Controllers.HeThong
{
    [Consumes("application/json")]
    [Route("api/HeThong/HT_BACKUP")]
    [ApiController]
    public class HT_BACKUPController : BaseController
    {
        public HT_BACKUPController(GDBContext db, IWebHostEnvironment hostingEnvironment, IConfiguration configuration) : base(db, hostingEnvironment, configuration)
        {
            NhomChucNang = NhomChucNang.QuanTriHeThong;
            NhomQuyen = Resource.QuyenHT_BACKUP;
        }
        public static List<QUYEN> Permission()
        {
            HT_BACKUPController mn = new HT_BACKUPController(null, null, null);
            return mn.QuyenCoBan(Quyen.SaoLuu, Quyen.PhucHoi);
        }

        [HttpGet("{op}")]
        [Authorize("Bearer")]
        public IActionResult Get(string op)
        {
            if (op == "Access")
            {
                return ObjectResult(new
                {
                    View = UserAccess(Quyen.SaoLuu) || UserAccess(Quyen.PhucHoi),
                    Title = CurrentMenu?.NAME + "",
                });
            }
            else if (op == "List")
            {
                var PathJson = Path.Combine(_hostingEnvironment.ContentRootPath, "Backup.json");
                var json = "";
                if (!System.IO.File.Exists(PathJson))
                {
                    FileStream fs = System.IO.File.Create(PathJson);
                    fs.Close();
                }
                else json = System.IO.File.ReadAllText(PathJson);
                List<HT_BACKUP_Info> listData = new List<HT_BACKUP_Info>();
                if (json != "") listData = JsonConvert.DeserializeObject<List<HT_BACKUP_Info>>(json);
                return ListAll(listData.AsQueryable());
            }
            return new BadRequestResult();
        }

        [HttpPost("{op}")]
        [Authorize("Bearer")]
        public IActionResult Post(string op, [FromBody] JObject item)
        {
            if (item == null || op == "") return new BadRequestResult();
            if (op == "Saoluu")
            {
                if (!UserAccess(Quyen.SaoLuu)) return UserAccessDenied();
                var PathJson = Path.Combine(_hostingEnvironment.ContentRootPath, "Backup.json");
                string duongdan = _configuration.GetValue<string>("AppSetings:PATH_BACKUP");
                string dateNow = DateTime.Now.ToString("ddMMyyyyHHmmss");
                string fileName = "Backup_" + dateNow + ".bak";
                string pathFull = Path.Combine(duongdan, fileName);
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("GDatabase")))
                {
                    fileName = con.Database + "_" + dateNow + ".bak";
                    pathFull = Path.Combine(duongdan, fileName);
                    using (SqlCommand cmd = new SqlCommand("HETHONG$sp_Backup", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Backup_Path", SqlDbType.NVarChar).Value = pathFull;
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                List<HT_BACKUP_Info> listData = new List<HT_BACKUP_Info>();
                string json = System.IO.File.ReadAllText(PathJson);
                if (json != "") listData = JsonConvert.DeserializeObject<List<HT_BACKUP_Info>>(json);
                var objBackup = new HT_BACKUP_Info
                {
                    ID = Guid.NewGuid().ToString(),
                    TEN_FILE = fileName,
                    NGAY_SAO_LUU = DateTime.Now,
                    NGAY_PHUC_HOI = null,
                    DUONG_DAN = pathFull,
                    NGUOIDUNG_ID = CurrentUser.ID
                };
                listData.Add(objBackup);
                System.IO.File.WriteAllText(PathJson, JsonConvert.SerializeObject(listData));
            }
            if (op == "PhucHoi")
            {
                if (!UserAccess(Quyen.PhucHoi)) return UserAccessDenied();
                var ID = item.GetValue("items").Value<string>();
                if (ID != "" || ID != null)
                {
                    var PathJson = Path.Combine(_hostingEnvironment.ContentRootPath, "Backup.json");
                    string duongdan = _configuration.GetValue<string>("AppSetings:PATH_BACKUP");
                    List<HT_BACKUP_Info> listData = new List<HT_BACKUP_Info>();
                    string json = System.IO.File.ReadAllText(PathJson);
                    if (json != "") listData = JsonConvert.DeserializeObject<List<HT_BACKUP_Info>>(json);
                    var objData = listData.Where(e => e.ID == ID).FirstOrDefault();

                    if (objData != null)
                    {

                        string pathFull = Path.Combine(duongdan, objData.TEN_FILE);
                        // restore
                        using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("GDatabase")))
                        {
                            con.Open();
                            string dbName = con.Database;
                            string UseMaster = "USE master";
                            SqlCommand UseMasterCommand = new SqlCommand(UseMaster, con);
                            UseMasterCommand.ExecuteNonQuery();

                            string Alter1 = @"ALTER DATABASE [" + dbName + "] SET Single_User WITH Rollback Immediate";
                            SqlCommand Alter1Cmd = new SqlCommand(Alter1, con);
                            Alter1Cmd.ExecuteNonQuery();

                            string Restore = string.Format("Restore database [{1}] from disk='{0}' WITH REPLACE", pathFull, dbName);
                            SqlCommand RestoreCmd = new SqlCommand(Restore, con);
                            RestoreCmd.ExecuteNonQuery();

                            string Alter2 = @"ALTER DATABASE [" + dbName + "] SET Multi_User";
                            SqlCommand Alter2Cmd = new SqlCommand(Alter2, con);
                            Alter2Cmd.ExecuteNonQuery();
                            con.Close();

                        }
                        // add file json
                        objData.NGAY_PHUC_HOI = DateTime.Now;
                        System.IO.File.WriteAllText(PathJson, JsonConvert.SerializeObject(listData));
                    }
                }

            }

            return new NoContentResult();
        }
    }
}
