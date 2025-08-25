using API.Common;
using GCommon;
using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace API.Controllers.DanhMuc
{
    [Consumes("application/json")]
    [Route("api/DanhMuc/DM_DANHMUC_IMPORT")]
    [ApiController]
    public class DM_DANHMUC_IMPORTController : BaseController
    {
        public DM_DANHMUC_IMPORTController(GDBContext db) : base(db)
        {
            NhomChucNang = NhomChucNang.DanhMucDungChung;
            NhomQuyen = Resource.QuyenDM_DANHMUC;
        }

        [HttpGet("{op}")]
        [Authorize("Bearer")]
        public IActionResult Get(string op)
        {
            if (op == "Access")
            {
                var objDM_DANHMUC = _db.DM_DANHMUCCollection.GetByMA(Request.Query["MA_DANHMUC"]);
                bool access = UserAccess(Quyen.Them);
                return ObjectResult(new
                {
                    View = access,
                    New = access,
                    Edit = access,
                    Delete = access,
                    Title = objDM_DANHMUC?.TEN + "",
                    danhmuc = objDM_DANHMUC
                });
            }
            return new BadRequestResult();
        }

        [HttpPost("{op}")]
        [Authorize("Bearer")]
        public IActionResult Post(string op, [FromBody] JObject item)
        {
            if (item == null || op == "") return new BadRequestResult();
            if (op == "Save")
            {
                if (!UserAccess(Quyen.Them)) return UserAccessDenied();
                var DANHMUC_ID = Convert.ToInt64(item["DANHMUC_ID"] + "");
                if (item["DANHSACH"] + "" != "")
                {
                    var lines = (item["DANHSACH"] + "").Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        var l = line.Trim();
                        var s = l.Split('|', StringSplitOptions.RemoveEmptyEntries);
                        DM_DANHMUC_ITEM dm = new DM_DANHMUC_ITEM();
                        dm.DANHMUC_ID = DANHMUC_ID;
                        dm.HETHONG = false;
                        if (s.Length == 1)
                        {
                            dm.TEN = s[0];
                        }
                        else if (s.Length == 2)
                        {
                            dm.MA = s[0];
                            dm.TEN = s[1];
                        }
                        else if (s.Length == 3)
                        {
                            dm.MA = s[0];
                            dm.TEN = s[1];
                            dm.PID = ConvertClass.ToLong(s[2], 0);
                        }
                        dm.TRANG_THAI = (int)TrangThai.DA_DUYET;
                        dm.SAP_XEP = 0;
                        dm.HASCHILD = false;
                        //dm.PID = 0;
                        dm.DVSD_ID = DVSD_ID;
                        dm.DVQL_ID = DVQL_ID;
                        dm.DATA = "{}";
                        _db.DM_DANHMUC_ITEMCollection.Add(dm);
                    }
                    UpdateHasChild("DM_DANHMUC_ITEM", "PID");
                }
            }
            return new NoContentResult();
        }
    }
}
