using API.Common;
using GDB;
using GCommon;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace API.Controllers.ThietBi
{
    [Consumes("application/json")]
    [Route("api/ThietBi/TB_KEYPM")]
    [ApiController]
    public class TB_KEYPMController : BaseController
    {
        public TB_KEYPMController(GDBContext db, IWebHostEnvironment hostingEnvironment, IConfiguration configuration) : base(db, hostingEnvironment, configuration)
        {
            NhomChucNang = NhomChucNang.ThietBi;
            NhomQuyen = Resource.QuyenTB_KEYPM;
        }

        public static List<QUYEN> Permission()
        {
            TB_KEYPMController mn = new TB_KEYPMController(null, null, null);
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
                    Title = CurrentMenu?.NAME + "",
                });
            }
            else if (op == "List")
            {
                return ListAll(_db.TB_KEYPMCollection.Get(DVQL_ID, DVSD_ID));
            }
            else if (op == "thietbi")
            {
                long KEYPM_ID = ConvertClass.ToLong(Request.Query["KEYPM_ID"] + "", 0);
                var lstMUONTRA_CT = _db.TB_KEYPMCTCollection.GetByKEYPM_ID(KEYPM_ID);
               // var lstMUONTRA_CT = _db.TB_KEYPMCTCollection.GetInfo(KEYPM_ID);
                return ObjectResult(lstMUONTRA_CT);

            }
            else if (op == "Thietbitrangcap")
            {
                return ListAll(_db.TB_THIETBICollection.Get(DVQL_ID, DVSD_ID).Where(e => e.PHAN_LOAI == (int)PhanLoai.TrangCap));
            }
            
            return new BadRequestResult();
        }
        //post
        [HttpPost("{op}")]
        [Authorize("Bearer")]
        public IActionResult Post(string op, [FromBody] JObject item)
        {
            if (item == null || op == "") return new BadRequestResult();
            if (op == "Create")
            {
                if (!UserAccess(Quyen.Them)) return UserAccessDenied();
                var itemNew = item.ToObject<TB_KEYPM>();
                itemNew.NGAY_TAO = DateTime.Now;
                itemNew.NGUOIDUNG_ID = CurrentUser.ID;
                itemNew.DVQL_ID = DVQL_ID;
                itemNew.DVSD_ID = DVSD_ID;
                _db.TB_KEYPMCollection.Add(itemNew);

                if (item["TB_MUONCT"].Count() != 0)
                {
                    var TB_MUONCT = JsonConvert.DeserializeObject<JArray>(item["TB_MUONCT"] + "");
                    List<TB_KEYPMCT> lstTB_MUON = new List<TB_KEYPMCT>();
                    foreach (var objCT in TB_MUONCT)
                    {
                       
                        var insertCT = objCT.ToObject<TB_KEYPMCT>();
                        insertCT.ID = 0;
                        insertCT.KEYPM_ID = itemNew.ID;
                        insertCT.NGAY_NHAP = DateTime.Now;
                        insertCT.DA_NHAP = ConvertClass.ToBoolean(objCT["DA_NHAP"] + "");

                        lstTB_MUON.Add(insertCT);
                    }
                    _db.TB_KEYPMCTCollection.Add(lstTB_MUON);
                }

                HtLog(Quyen.Them, itemNew.ID, "Nhập quản lý key bản quyền " + itemNew.ID);

            }
            else if (op == "Delete")
            {
                if (!UserAccess(Quyen.Xoa)) return UserAccessDenied();
                var jItem = JsonConvert.DeserializeObject<JArray>(item.GetValue("items").Value<string>());
                foreach (var id in jItem)
                {
                    var objMUONTRA = _db.TB_KEYPMCollection.GetByID(id.Value<long>());
                    _db.TB_KEYPMCTCollection.RemoveByKEYPM_ID(objMUONTRA.ID);
                    _db.TB_KEYPMCollection.Remove(objMUONTRA);
                    HtLog(Quyen.Xoa, objMUONTRA.ID);
                }
            }
            return new NoContentResult();
        }
        //edit
        [HttpPut("{ID}")]
        [Authorize("Bearer")]
        public IActionResult Put(long ID, [FromBody] JObject item)
        {

            var objTB_KEYPM = _db.TB_KEYPMCollection.GetByID(ID);

            if (objTB_KEYPM != null)
            {
                if (!UserAccess(Quyen.Sua)) return UserAccessDenied();
                if (objTB_KEYPM.DVSD_ID != DVSD_ID) return UserAccessDenied();

                UpdateObject(objTB_KEYPM, item, new string[] { "id" });
                objTB_KEYPM.NGAY_SUA = DateTime.Now;
                _db.TB_KEYPMCollection.Update(objTB_KEYPM);

                var lstTB_MUONCT = _db.TB_KEYPMCTCollection.GetByKEYPM_ID(ID);
                if (lstTB_MUONCT.Count() > 0) _db.TB_KEYPMCTCollection.Remove(lstTB_MUONCT);
                if (item["TB_MUONCT"].Count() != 0)
                {
                    var TB_MUONCT = JsonConvert.DeserializeObject<JArray>(item["TB_MUONCT"] + "");
                    List<TB_KEYPMCT> lstTB_MUON = new List<TB_KEYPMCT>();
                    foreach (var objCT in TB_MUONCT)
                    {
                        
                        var insertCT = objCT.ToObject<TB_KEYPMCT>();
                        insertCT.ID = 0;
                        insertCT.KEYPM_ID = ID;
                        insertCT.NGAY_NHAP = DateTime.Now;
                        insertCT.DA_NHAP = ConvertClass.ToBoolean(objCT["DA_NHAP"] + "");
                        lstTB_MUON.Add(insertCT);
                    }
                    _db.TB_KEYPMCTCollection.Add(lstTB_MUON);
                }

                HtLog(Quyen.Sua, objTB_KEYPM.ID);
            }
            return new NoContentResult();
        }
    }
}
