using API.Common;
using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using GCommon;

namespace API.Controllers.HeThong
{
    [Consumes("application/json")]
    [Route("api/HeThong/HT_NHOMQUYEN_QUYEN")]
    [ApiController]
    public class HT_NHOMQUYEN_QUYENController : BaseController
    {
        public HT_NHOMQUYEN_QUYENController(GDBContext db) : base(db)
        {
            NhomChucNang = NhomChucNang.QuanTriHeThong;
            NhomQuyen = Resource.QuyenHT_NHOMQUYEN;
        }

        [HttpGet("{op}")]
        [Authorize("Bearer")]
        public IActionResult Get(string op)
        {
            if (op == "Access")
            {
                return ObjectResult(new
                {
                    View = UserAccess(Quyen.PhanQuyen),
                });
            }
            else if (op == "ListQUYEN")
            {
                if (IsAdmin)
                {
                    return ListAll(GetSystemPermissionTree().AsQueryable());
                }
                else
                {
                    var lstQuyen = GetPermission().Select(s => s.QUYEN);
                    return ListAll(GetSystemPermissionTree(lstQuyen.ToList()).AsQueryable());
                }                
            }
            else if (op == "List")
            {
                long RID = ConvertClass.ToLong(Request.Query["RID"] + "", 0);
                return ObjectResult(new { items = _db.HT_DOITUONG_QUYENCollection.GetByDOITUONG_ID(RID, DsDoiTuong.DM_DANHMUC, DsChucNang.NhomQuyen.ToString()) });
            }
            return new BadRequestResult();
        }

        [HttpPost("{op}")]
        [Authorize("Bearer")]
        public IActionResult Post(string op, [FromBody] JObject item)
        {
            if (!UserAccess(Quyen.PhanQuyen)) return UserAccessDenied();
            if (item == null || op == "") return new BadRequestResult();
            if (op == "Create")
            {
                long RID = ConvertClass.ToLong(item["RID"]+"", 0);
                List<HT_DOITUONG_QUYEN> lstInsert = new List<HT_DOITUONG_QUYEN>();
                List<HT_DOITUONG_QUYEN> lstDelete = new List<HT_DOITUONG_QUYEN>();
                var objHT_NHOMQUYEN = _db.DM_DANHMUC_ITEMCollection.GetByID(RID); //_db.HT_VARIABLECollection.HT_NHOMQUYEN_GetByID(RID);
                if(objHT_NHOMQUYEN.DVQL_ID != DVQL_ID) return UserAccessDenied();
                if (!IsQuanTriHeThong && objHT_NHOMQUYEN.DVSD_ID != DVSD_ID) return UserAccessDenied();
                var lstHT_DOITUONG_QUYEN = _db.HT_DOITUONG_QUYENCollection.GetByDOITUONG_ID(RID, DsDoiTuong.DM_DANHMUC, DsChucNang.NhomQuyen.ToString()).ToList();
                var jItem = JsonConvert.DeserializeObject<List<string>>(item["items"] + "");
                List<string> lstQuyen = GetSystemPermission().Select(s => s.MA).ToList();
                foreach (var pid in jItem)
                {
                    if (!lstQuyen.Contains(pid)) continue;
                    var find = lstHT_DOITUONG_QUYEN.FirstOrDefault(c => c.QUYEN == pid + "") != null;
                    if (!find)
                        lstInsert.Add(new HT_DOITUONG_QUYEN { DOITUONG_ID = RID, DOITUONG_LOAI = DsDoiTuong.DM_DANHMUC, CHUCNANG = DsChucNang.NhomQuyen.ToString(), QUYEN = pid + "" });
                }
                var lstQuyenInsert = lstInsert.Select(s => s.QUYEN).ToList();
                lstDelete = lstHT_DOITUONG_QUYEN.Where(c => !lstQuyenInsert.Contains(c.QUYEN) && !jItem.Contains(c.QUYEN)).ToList();
                _db.HT_DOITUONG_QUYENCollection.AddBulk(lstInsert);
                _db.HT_DOITUONG_QUYENCollection.RemoveBulk(lstDelete);
                _db.SaveChanges();
                HtLog(Quyen.PhanQuyen, RID);
            }
            return new NoContentResult();
        }
    }
}
